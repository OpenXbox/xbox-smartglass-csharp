using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Model;
using SmartGlass.Tests.Resources;
using Xunit;

namespace SmartGlass.Tests
{
    public class TestConsoleCache
    {
        readonly string _consoleListJsonFilepath;
        readonly string _consoleListJson;
        readonly string _singleConsoleJsonFilepath;
        readonly string _singleConsoleJson;
        Device _knownDevice;
        Device _newDevice;

        public TestConsoleCache()
        {
            _consoleListJsonFilepath = ResourcesProvider.GetFilePath("console_list.json", ResourceType.Json);
            _consoleListJson = ResourcesProvider.GetString("console_list.json", ResourceType.Json);
            _singleConsoleJsonFilepath = ResourcesProvider.GetFilePath("console_list.json", ResourceType.Json);
            _singleConsoleJson = ResourcesProvider.GetString("console.json", ResourceType.Json);

            _knownDevice = new Device(
                DeviceType.XboxOne,
                IPAddress.Parse("192.168.1.3"),
                "XboxOne",
                "FD0123456789",
                new Guid("de305d54-75b4-431b-adb2-eb6b9e546014")
            );

            _newDevice = new Device(
                DeviceType.XboxOne,
                IPAddress.Parse("192.168.1.123"),
                "SomeName",
                "FD9023437589",
                new Guid("deadbeef-ffaa-4eee-adb2-ff6b9e547799")
            );
        }

        [Fact]
        public void TestLoadFromJson()
        {
            var obj = ConsoleCache.LoadFromJson(_consoleListJson);

            Assert.NotNull(obj.Devices);
            Assert.Equal(2, obj.Devices.Count);

            var firstDevice = obj.Devices[0];
            Assert.Equal(firstDevice.DeviceType, _knownDevice.DeviceType);
            Assert.Equal(firstDevice.HardwareId, _knownDevice.HardwareId);
            Assert.Equal(firstDevice.LiveId, _knownDevice.LiveId);
            Assert.Equal(firstDevice.Address, _knownDevice.Address);
            Assert.Equal(firstDevice.Name, _knownDevice.Name);
            
        }

        [Fact]
        public void TestLoadFromJsonFile()
        {
            var obj = ConsoleCache.LoadFromJsonFile(_consoleListJsonFilepath);

            Assert.NotNull(obj.Devices);
            Assert.Equal(2, obj.Devices.Count);

            var firstDevice = obj.Devices[0];
            Assert.Equal(firstDevice.DeviceType, _knownDevice.DeviceType);
            Assert.Equal(firstDevice.HardwareId, _knownDevice.HardwareId);
            Assert.Equal(firstDevice.LiveId, _knownDevice.LiveId);
            Assert.Equal(firstDevice.Address, _knownDevice.Address);
            Assert.Equal(firstDevice.Name, _knownDevice.Name);
        }

        [Fact]
        public void TestUpdateInstanceFromJsonFile()
        {
            var obj = new ConsoleCache();
            obj.UpdateFromJsonFile(_consoleListJsonFilepath);

            Assert.NotNull(obj.Devices);
            Assert.Equal(2, obj.Devices.Count);

            var firstDevice = obj.Devices[0];
            Assert.Equal(firstDevice.DeviceType, _knownDevice.DeviceType);
            Assert.Equal(firstDevice.HardwareId, _knownDevice.HardwareId);
            Assert.Equal(firstDevice.LiveId, _knownDevice.LiveId);
            Assert.Equal(firstDevice.Address, _knownDevice.Address);
            Assert.Equal(firstDevice.Name, _knownDevice.Name);
        }

        [Fact]
        public void TestDumpToJson()
        {
            var obj = ConsoleCache.LoadFromJsonFile(_consoleListJsonFilepath);
            var tempFilePath = ResourcesProvider.GetRandomTempFilePath();

            bool success = obj.DumpToJsonFile(tempFilePath);
            Assert.True(success);
        }

        [Fact]
        public void TestAddConsole()
        {
            var obj = ConsoleCache.LoadFromJsonFile(_consoleListJsonFilepath);

            Assert.Equal(2, obj.Devices.Count);
            obj.AddDevice(_newDevice);
            Assert.Equal(3, obj.Devices.Count);
            obj.AddDevice(_newDevice);
            Assert.Equal(3, obj.Devices.Count);
        }

        [Fact]
        public void TestRemoveConsole()
        {
            var obj = ConsoleCache.LoadFromJsonFile(_consoleListJsonFilepath);

            bool success = false;
            Assert.Equal(2, obj.Devices.Count);
            
            // Remove unknown device
            success = obj.RemoveDevice(_newDevice);
            Assert.False(success);
            Assert.Equal(2, obj.Devices.Count);
            
            // Remove known device
            success = obj.RemoveDevice(_knownDevice);
            Assert.True(success);
            Assert.Single(obj.Devices);
        }
    }
}