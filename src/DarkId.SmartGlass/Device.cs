using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Messaging;
using DarkId.SmartGlass.Messaging.Discovery;
using DarkId.SmartGlass.Messaging.Power;
using Org.BouncyCastle.X509;

namespace DarkId.SmartGlass
{
    public class Device
    {
        private static readonly TimeSpan discoveryListenTime = TimeSpan.FromSeconds(1);

        private static readonly TimeSpan pingTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan[] pingRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(1500),
            TimeSpan.FromSeconds(5)
        };

        public static Task<IEnumerable<Device>> DiscoverAsync()
        {
            return Task.Run(() =>
            {
                using (var messageTransport = new MessageTransport())
                {
                    var requestMessage = new PresenceRequestMessage();

                    return messageTransport.ReadMessages(discoveryListenTime,
                        () => messageTransport.SendAsync(requestMessage).Wait())
                        .OfType<PresenceResponseMessage>()
                        .DistinctBy(m => m.HardwareId)
                        .Select(m => new Device(m)).ToArray().AsEnumerable();
                }
            });
        }

        public static async Task<Device> PingAsync(string addressOrHostname)
        {
            using (var messageTransport = new MessageTransport(addressOrHostname))
            {
                var requestMessage = new PresenceRequestMessage();

                var response = await TaskExtensions.WithRetries(() =>
                    messageTransport.WaitForMessageAsync<PresenceResponseMessage>(pingTimeout,
                    () => messageTransport.SendAsync(requestMessage).Wait()),
                        pingRetries);

                return new Device(response);
            }
        }

        public static async Task<Device> PowerOnAsync(string addressOrHostname, string liveId, int times = 5, int delay = 1000)
        {
            using (var messageTransport = new MessageTransport(addressOrHostname))
            {
                var requestMessage = new PowerOnMessage { LiveId = liveId };

                for (var i = 0; i < times; i++)
                {
                    await messageTransport.SendAsync(requestMessage);
                    await Task.Delay(delay);
                }

                return await PingAsync(addressOrHostname);
            }
        }

        public IPAddress Address { get; private set; }
        public DeviceFlags Flags { get; private set; }
        public DeviceType DeviceType { get; private set; }
        public string Name { get; private set; }
        public Guid HardwareId { get; private set; }
        public X509Certificate Certificate { get; private set; }

        private Device(PresenceResponseMessage message)
        {
            Address = message.Origin.Address;
            Flags = message.Flags;
            DeviceType = message.DeviceType;
            Name = message.Name;
            HardwareId = message.HardwareId;
            Certificate = message.Certificate;
        }
    }
}