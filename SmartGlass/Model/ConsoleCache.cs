using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartGlass.Common;

namespace SmartGlass.Model
{
    /// <summary>
    /// The ConsoleCache is used to store discovered consoles.
    /// </summary>
    public class ConsoleCache
    {
        private static ILogger logger = Logging.Factory.CreateLogger<ConsoleCache>();

        /// <summary>
        /// Cached console devices.
        /// </summary>
        /// <value></value>
        public List<Device> Devices { get; private set; }

        /// <summary>
        /// Load devices from a JSON string.
        /// </summary>
        /// <param name="json">JSON string.</param>
        /// <returns></returns>
        public static ConsoleCache LoadFromJson(string json)
        {
            logger.LogTrace("ConsoleCache.LoadFromJson called with JSON data: {}", json);
            var devices = JsonConvert.DeserializeObject<List<Device>>(
                json, Device.GetJsonSerializerSettings());
            logger.LogDebug("ConsoleCache.LoadFromJson: Deserialized {} items", devices.Count);
            
            return new ConsoleCache()
            {
                Devices = devices
            };
        }

        /// <summary>
        /// Load devices from a JSON file.
        /// </summary>
        /// <param name="sourceFilePath">Filepath of JSON file to load.</param>
        /// <returns>Returns a ConsoleCache instance.</returns>
        public static ConsoleCache LoadFromJsonFile(string sourceFilePath)
        {
            logger.LogTrace("ConsoleCache.LoadFromJsonFile called with sourceFilePath: {}", sourceFilePath);
            var json = File.ReadAllText(sourceFilePath, System.Text.Encoding.UTF8);
            logger.LogTrace("ConsoleCache.LoadFromJsonFile: File.ReadAllText read JSON: {}", json);
            return LoadFromJson(json);
        }

        /// <summary>
        /// Dump all devices to a JSON string.
        /// </summary>
        /// <param name="obj">ConsoleCache object to dump.</param>
        /// <returns>Returns a serialized JSON string.</returns>
        public static string DumpToJson(ConsoleCache obj)
        {
            logger.LogTrace("ConsoleCache.DumpToJson called");
            return JsonConvert.SerializeObject(obj.Devices, Device.GetJsonSerializerSettings());
        }

        /// <summary>
        /// Dump all devices to a JSON file.
        /// </summary>
        /// <param name="obj">ConsoleCache object to dump.</param>
        /// <param name="targetFilePath">Filepath to write to.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public static bool DumpToJsonFile(ConsoleCache obj, string targetFilePath)
        {
            logger.LogTrace("ConsoleCache.DumpToJsonFile called");
            try
            {
                var json = DumpToJson(obj);
                logger.LogTrace("DumpToJsonFile: Returned JSON from DumpToJson: {}", json);
                File.WriteAllText(targetFilePath, json, System.Text.Encoding.UTF8);
                logger.LogTrace("DumpToJsonFile: Wrote successfully to file: {}", targetFilePath);
                return true;
            }
            catch (Exception exc)
            {
                logger.LogError("DumpToJsonFile: Failed to dump json to file, error: {}", exc);
                return false;
            }
        }

        /// <summary>
        /// ConsoleCache default constructor.
        /// Instantiates an empty list of devices.
        /// </summary>
        public ConsoleCache()
        {
            logger.LogTrace("ConsoleCache default ctor called");
            Devices = new List<Device>();
        }

        /// <summary>
        /// ConsoleCache constructor.
        /// </summary>
        /// <param name="devices"></param>
        public ConsoleCache(List<Device> devices)
        {
            logger.LogTrace("ConsoleCache ctor(List<Device> devices) called");
            Devices = devices;
        }

        /// <summary>
        /// Adds a device.
        /// If a device with same hardware id already exists, it gets overwritten.
        /// </summary>
        /// <returns>Nothing</returns>
        public void AddDevice(Device device)
        {
            logger.LogTrace("ConsoleCache.AddDevice called, device: {}", device);
            int index = Devices.FindIndex(d => d.HardwareId == device.HardwareId);
            logger.LogTrace("ConsoleCache.AddDevice: Devices.FindIndex returned: {}", index);
            if (index == -1)
            {
                logger.LogDebug("ConsoleCache.AddDevice: Adding new device, hardware id: {}", device.HardwareId);
                Devices.Add(device);
            }
            else
            {
                logger.LogDebug("ConsoleCache.AddDevice: Replacing existing device, hardware id: {}", device.HardwareId);
                Devices[index] = device;
            }
        }

        /// <summary>
        /// Removes a device.
        /// Entry to remove is determined by hardware id.
        /// </summary>
        /// <returns>Returns true on success, false otherwise</returns>
        public bool RemoveDevice(Device device)
        {
            logger.LogTrace("ConsoleCache.RemoveDevice called, device: {}", device);
            int index = Devices.FindIndex(d => d.HardwareId == device.HardwareId);
            logger.LogTrace("ConsoleCache.RemoveDevice: Devices.FindIndex returned: {}", index);
            if (index == -1)
            {
                logger.LogDebug("ConsoleCache.RemoveDevice: No device found with hardware id: {}", device.HardwareId);
                return false;
            }

            logger.LogDebug("ConsoleCache.RemoveDevice: Removing device with hardware id: {}", device.HardwareId);
            Devices.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Shorthand version of DumpToJsonFile for an object instance.
        /// </summary>
        /// <param name="targetFilePath">Filepath to write to.</param>
        /// <returns>Returns true on success, false otherwise</returns>
        public bool DumpToJsonFile(string targetFilePath)
            => DumpToJsonFile(this, targetFilePath);
        
        /// <summary>
        /// Update current ConsoleCache instance via JSON file
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns>Return true on success, false otherwise.</returns>
        public bool UpdateFromJsonFile(string sourceFilePath)
        {
            logger.LogTrace("ConsoleCache.UpdateFromJsonFile called");
            ConsoleCache tempObj = null;
            try
            {
                logger.LogTrace("ConsoleCache.UpdateFromJsonFile: Calling LoadFromJsonFile");
                tempObj = LoadFromJsonFile(sourceFilePath);
            }
            catch (Exception exc)
            {
                logger.LogError("ConsoleCache.UpdateFromJsonFile: LoadFromJsonFile failed, error: {}", exc);
                return false;
            }
            logger.LogTrace("ConsoleCache.UpdateFromJsonFile: Loaded {} devices", tempObj.Devices.Count);
            foreach(var device in tempObj.Devices)
            {
                logger.LogDebug("Adding device: {}", device);
                this.AddDevice(device);
            }
            return true;
        }
    }
}