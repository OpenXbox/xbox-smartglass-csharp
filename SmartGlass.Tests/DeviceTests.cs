using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using SmartGlass.Common;
using SmartGlass.Connection;
using SmartGlass.Messaging.Discovery;
using SmartGlass.Tests.Resources;
using Xunit;

namespace SmartGlass.Tests
{
    public class DeviceTests
    {
        PresenceResponseMessage _presenceResponse;
        Device _deviceManual;
        Device _deviceFromResponse;

        public DeviceTests()
        {
            byte[] cert = ResourcesProvider.GetBytes("selfsigned_cert.bin", ResourceType.Misc);
            byte[] msgBytes = ResourcesProvider.GetBytes("presence_response.bin", ResourceType.SmartGlass);
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);
            
            _presenceResponse = new PresenceResponseMessage();
            using (EndianReader er = new EndianReader(msgBytes))
            {
                _presenceResponse.Deserialize(er);
            }
            _presenceResponse.Origin = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 5234);

            _deviceManual = new Device(
                DeviceType.XboxOne,
                IPAddress.Parse("192.168.1.3"),
                "XboxOne",
                "FD0123456789",
                new Guid("de305d54-75b4-431b-adb2-eb6b9e546014")
            );

            _deviceFromResponse = new Device(_presenceResponse);
        }

        [Fact]
        public void TestManualInit()
        {
            Assert.Equal(IPAddress.Parse("192.168.1.3"), _deviceManual.Address);
            Assert.Equal(DeviceType.XboxOne, _deviceManual.DeviceType);
            Assert.Equal("XboxOne", _deviceManual.Name);
            Assert.Equal(new Guid("de305d54-75b4-431b-adb2-eb6b9e546014"), _deviceManual.HardwareId);
            Assert.Equal("FD0123456789", _deviceManual.LiveId);
            Assert.Equal(DeviceState.Unavailable, _deviceManual.State);
        
            Assert.Equal(DeviceFlags.None, _deviceManual.Flags);
            Assert.Null(_deviceManual.Certificate);
        }

        [Fact]
        public void TestPresenceResponseInit()
        {
            Assert.Equal(IPAddress.Parse("192.168.1.2"), _deviceFromResponse.Address);
            Assert.Equal(DeviceType.XboxOne, _deviceFromResponse.DeviceType);
            Assert.Equal("XboxOne", _deviceFromResponse.Name);
            Assert.Equal(new Guid("de305d54-75b4-431b-adb2-eb6b9e546014"), _deviceFromResponse.HardwareId);
            Assert.Equal("FFFFFFFFFFF", _deviceFromResponse.LiveId);
            Assert.Equal(DeviceState.Available, _deviceFromResponse.State);

            Assert.Equal(DeviceFlags.AllowAuthenticatedUsers, _deviceFromResponse.Flags);
            Assert.NotNull(_deviceFromResponse.Certificate);
        }

        [Fact]
        public void TestManualJsonSerialization()
        {
            var json = JsonConvert.SerializeObject(_deviceManual, Device.GetJsonSerializerSettings());
            
            Assert.Contains("\"address\": \"192.168.1.3\"", json);
            Assert.Contains("\"type\": 1", json);
            Assert.Contains("\"name\": \"XboxOne\"", json);
            Assert.Contains("\"liveid\": \"FD0123456789\"", json);
            Assert.Contains("\"uuid\": \"de305d54-75b4-431b-adb2-eb6b9e546014\"", json);
        }

        [Fact]
        public void TestPresenceResponseJsonSerialization()
        {
            var json = JsonConvert.SerializeObject(_deviceFromResponse, Device.GetJsonSerializerSettings());
            
            Assert.Contains("\"address\": \"192.168.1.2\"", json);
            Assert.Contains("\"type\": 1", json);
            Assert.Contains("\"name\": \"XboxOne\"", json);
            Assert.Contains("\"liveid\": \"FFFFFFFFFFF\"", json);
            Assert.Contains("\"uuid\": \"de305d54-75b4-431b-adb2-eb6b9e546014\"", json);
        }

        [Fact]
        public void TestDeserializationSingle()
        {
            var json = ResourcesProvider.GetString("console.json", ResourceType.Json);
            var dev = JsonConvert.DeserializeObject<Device>(json, Device.GetJsonSerializerSettings());

            Assert.Equal(dev.Address, _deviceManual.Address);
            Assert.Equal(dev.DeviceType, _deviceManual.DeviceType);
            Assert.Equal(dev.Name, _deviceManual.Name);
            Assert.Equal(dev.LiveId, _deviceManual.LiveId);
            Assert.Equal(dev.HardwareId, _deviceManual.HardwareId);
        }

        [Fact]
        public void TestDeserializationList()
        {
            var json = ResourcesProvider.GetString("console_list.json", ResourceType.Json);
            var devs = JsonConvert.DeserializeObject<IEnumerable<Device>>(json, Device.GetJsonSerializerSettings());
            var dev = devs.First();

            Assert.Equal(dev.Address, _deviceManual.Address);
            Assert.Equal(dev.DeviceType, _deviceManual.DeviceType);
            Assert.Equal(dev.Name, _deviceManual.Name);
            Assert.Equal(dev.LiveId, _deviceManual.LiveId);
            Assert.Equal(dev.HardwareId, _deviceManual.HardwareId);
        }

        [Fact]
        public void TestSerializeDeserialize()
        {
            var serialized = JsonConvert.SerializeObject(_deviceManual, Device.GetJsonSerializerSettings());
            var deserialized = JsonConvert.DeserializeObject<Device>(serialized, Device.GetJsonSerializerSettings());
        
            Assert.Equal(deserialized.Address, _deviceManual.Address);
            Assert.Equal(deserialized.DeviceType, _deviceManual.DeviceType);
            Assert.Equal(deserialized.Name, _deviceManual.Name);
            Assert.Equal(deserialized.LiveId, _deviceManual.LiveId);
            Assert.Equal(deserialized.HardwareId, _deviceManual.HardwareId);
        }
    }
}