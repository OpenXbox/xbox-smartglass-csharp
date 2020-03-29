using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartGlass.Common;

namespace SmartGlass.Model
{
    public class ClientConfig
    {
        private static ILogger logger = Logging.Factory.CreateLogger<ClientInfo>();

        /// <summary>
        /// Client UUID
        /// </summary>
        /// <value></value>
        public Guid ClientUuid { get; set; }
        
        /// <summary>
        /// Client information, used by LocalJoin message
        /// </summary>
        /// <value></value>
        public ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// Json serializer settings, used by Newtonsoft.Json
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Json.IPAddressConverter());
            settings.Formatting = Formatting.Indented;
            return settings;
        }

        /// <summary>
        /// Load client config from a JSON string.
        /// </summary>
        /// <param name="json">JSON string.</param>
        /// <returns></returns>
        public static ClientConfig LoadFromJson(string json)
        {
            logger.LogTrace("ClientConfig.LoadFromJson called with JSON data: {}", json);
            return JsonConvert.DeserializeObject<ClientConfig>(
                json, Device.GetJsonSerializerSettings());
        }

        /// <summary>
        /// Load client config from a JSON file.
        /// </summary>
        /// <param name="sourceFilePath">Filepath of JSON file to load.</param>
        /// <returns>Returns a ClientConfig instance.</returns>
        public static ClientConfig LoadFromJsonFile(string sourceFilePath)
        {
            logger.LogTrace("ClientConfig.LoadFromJsonFile called with sourceFilePath: {}", sourceFilePath);
            var json = File.ReadAllText(sourceFilePath, System.Text.Encoding.UTF8);
            logger.LogTrace("ClientConfig.LoadFromJsonFile: File.ReadAllText read JSON: {}", json);
            return LoadFromJson(json);
        }

        /// <summary>
        /// Dump client config to a JSON string.
        /// </summary>
        /// <param name="obj">ClientConfig object to dump.</param>
        /// <returns>Returns a serialized JSON string.</returns>
        public static string DumpToJson(ClientConfig obj)
        {
            logger.LogTrace("ClientConfig.DumpToJson called");
            return JsonConvert.SerializeObject(obj, GetJsonSerializerSettings());
        }

        /// <summary>
        /// Dump client config to a JSON file.
        /// </summary>
        /// <param name="obj">ClientConfig object to dump.</param>
        /// <param name="targetFilePath">Filepath to write to.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public static bool DumpToJsonFile(ClientConfig obj, string targetFilePath)
        {
            logger.LogTrace("ClientConfig.DumpToJsonFile called");
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
        /// 
        /// </summary>
        /// <param name="clientUuid">Client UUID</param>
        /// <param name="clientInfo">Client info</param>
        public ClientConfig(Guid clientUuid, ClientInfo clientInfo)
        {
            logger.LogTrace("ClientConfig ctor(Guid clientUuid, ClientInfo clientInfo) called");
            ClientUuid = clientUuid;
            ClientInfo = clientInfo;
        }

                /// <summary>
        /// Shorthand version of DumpToJsonFile for an object instance.
        /// </summary>
        /// <param name="targetFilePath">Filepath to write to.</param>
        /// <returns>Returns true on success, false otherwise</returns>
        public bool DumpToJsonFile(string targetFilePath)
            => DumpToJsonFile(this, targetFilePath);
        
        /// <summary>
        /// Update current ClientConfig instance via JSON file
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns>Return true on success, false otherwise.</returns>
        public bool UpdateFromJsonFile(string sourceFilePath)
        {
            logger.LogTrace("ClientConfig.UpdateFromJsonFile called");
            ClientConfig tempObj = null;
            try
            {
                logger.LogTrace("ClientConfig.UpdateFromJsonFile: Calling LoadFromJsonFile");
                tempObj = LoadFromJsonFile(sourceFilePath);
            }
            catch (Exception exc)
            {
                logger.LogError("ClientConfig.UpdateFromJsonFile: LoadFromJsonFile failed, error: {}", exc);
                return false;
            }

            logger.LogTrace("ClientConfig.UpdateFromJsonFile: Assigning values from JSON");
            ClientUuid = tempObj.ClientUuid;
            ClientInfo = tempObj.ClientInfo;
            return true;
        }
    }
}