using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Connection;
using SmartGlass.Messaging;
using SmartGlass.Messaging.Discovery;
using SmartGlass.Messaging.Power;
using Org.BouncyCastle.X509;
using Newtonsoft.Json;

namespace SmartGlass
{
    public class Device
    {
        private static readonly TimeSpan discoveryListenTime = TimeSpan.FromSeconds(1);

        private static readonly TimeSpan pingTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan[] pingRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(500)
        };

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Json.IPAddressConverter());
            settings.Formatting = Formatting.Indented;
            return settings;
        }

        public static Task<IEnumerable<Device>> DiscoverAsync(string liveId = null)
        {
            return Task.Run(() =>
            {
                using (var messageTransport = new MessageTransport())
                {
                    var requestMessage = new PresenceRequestMessage();

                    return messageTransport.ReadMessages(discoveryListenTime,
                        () => messageTransport.SendAsync(requestMessage))
                        .OfType<PresenceResponseMessage>()
                        .DistinctBy(m => m.HardwareId)
                        .Where(m => liveId == null || m.Certificate.GetLiveId() == liveId)
                        .Select(m => new Device(m)).ToArray().AsEnumerable();
                }
            });
        }

        public static async Task<Device> PingAsync(IPAddress address)
        {
            return await PingAsync(address.ToString());
        }

        public static async Task<Device> PingAsync(string addressOrHostname)
        {
            using (var messageTransport = new MessageTransport(addressOrHostname))
            {
                var requestMessage = new PresenceRequestMessage();

                var response = await Common.TaskExtensions.WithRetries(() =>
                    messageTransport.WaitForMessageAsync<PresenceResponseMessage>(pingTimeout,
                    () => messageTransport.SendAsync(requestMessage)),
                        pingRetries);

                return new Device(response);
            }
        }

        public static async Task<Device> PowerOnAsync(string liveId, int times = 5, int delay = 1000,
                                                      string addressOrHostname = null)
        {
            using (var messageTransport = new MessageTransport(addressOrHostname))
            {
                var poweronRequestMessage = new PowerOnMessage { LiveId = liveId };

                for (var i = 0; i < times; i++)
                {
                    await messageTransport.SendAsync(poweronRequestMessage);
                    await Task.Delay(delay);
                }

                var presenceRequestMessage = new PresenceRequestMessage();

                var response = await Common.TaskExtensions.WithRetries(() =>
                    messageTransport.WaitForMessageAsync<PresenceResponseMessage>(pingTimeout,
                    () => messageTransport.SendAsync(presenceRequestMessage)),
                        pingRetries);

                return new Device(response);
            }
        }

        [JsonProperty("address")]
        public IPAddress Address { get; private set; }
        
        [JsonProperty("type")]
        public DeviceType DeviceType { get; private set; }
        
        [JsonProperty("name")]
        public string Name { get; private set; }
        
        [JsonProperty("liveid")]
        public string LiveId { get; private set; }
        
        [JsonProperty("uuid")]
        public Guid HardwareId { get; private set; }
        
        [JsonIgnore]
        public DeviceFlags Flags { get; private set; }

        [JsonIgnore]
        public X509Certificate Certificate { get; private set; }
        
        [JsonIgnore]
        public DeviceState State { get; private set; }

        /// <summary>
        /// Initialize Device manually
        /// </summary>
        /// <param name="type">Type of device</param>
        /// <param name="address">IP address</param>
        /// <param name="name">Friendly name</param>
        /// <param name="liveId">Live ID</param>
        /// <param name="hardwareId">Hardware Id</param>
        public Device(DeviceType type, IPAddress address, string name, string liveId, Guid hardwareId)
        {
            Address = address;
            Flags = DeviceFlags.None;
            DeviceType = type;
            Name = name;
            HardwareId = hardwareId;
            Certificate = null;
            LiveId = liveId;

            State = DeviceState.Unavailable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Device"/>
        /// class by copying from an existing instance.
        /// NOTE: The new instance will have an initial state of
        /// DeviceState.Unavailable
        /// </summary>
        /// <param name="copyFrom">Device object to copy from.</param>
        public Device(Device copyFrom)
            : this(copyFrom.DeviceType, copyFrom.Address, copyFrom.Name,
                   copyFrom.LiveId, copyFrom.HardwareId)
        {
        }

        internal Device(PresenceResponseMessage message)
        {
            Address = message.Origin.Address;
            Flags = message.Flags;
            DeviceType = message.DeviceType;
            Name = message.Name;
            HardwareId = message.HardwareId;
            Certificate = message.Certificate;
            LiveId = message.Certificate.GetLiveId();

            State = DeviceState.Available;
        }

        /// <summary>
        /// Update instance's address, flags, name and certificate.
        /// Also sets DeviceState to "Available"
        /// </summary>
        /// <param name="newParameters">Freshly discovered device</param>
        void Update(Device newParameters)
        {
            if (LiveId != newParameters.LiveId)
                throw new InvalidOperationException();

            Address = newParameters.Address;
            Flags = newParameters.Flags;
            Name = newParameters.Name;
            Certificate = newParameters.Certificate;

            State = DeviceState.Available;
        }

        /// <summary>
        /// Power on console by sending a PowerOn packet via multicast
        /// </summary>
        /// <returns>true if poweron was successful, false otherwise</returns>
        public async Task<bool> PowerOnAsync()
        {
            var ipAddress = Address.ToString();
            try
            {
                var device = await Device.PowerOnAsync(LiveId);
                // Successfully powered on
                Update(device);
            }
            catch (TimeoutException)
            {
                // Try again via IP address
                try
                {
                    var device = await Device.PowerOnAsync(
                        LiveId, addressOrHostname: ipAddress);
                    Update(device);
                }
                catch (TimeoutException)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Ping console by LiveID and IP address.
        /// Update certificate, name, flags and address if necessary.
        /// On success, State is changed to "Available".
        /// </summary>
        /// <returns>true is console responded, false otherwise</returns>
        public async Task<bool> PingAsync()
        {
            try
            {
                // Try to reach console by last known IP address
                var deviceByAddress = await Device.PingAsync(Address.ToString());
                // Console found
                Update(deviceByAddress);
                return true;
            }
            catch (TimeoutException)
            {
                // pass
            }

            // Also try to discover via multicast presence request
            var deviceByLiveId = await Device.DiscoverAsync(LiveId);
            if (deviceByLiveId.Count() > 0)
            {
                // Console found via multicast
                Update(deviceByLiveId.First());    
                return true;
            }

            State = DeviceState.Unavailable;
            return false;
        }
    }
}
