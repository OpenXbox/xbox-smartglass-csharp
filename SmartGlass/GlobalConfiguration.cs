using System.Net;

namespace SmartGlass
{
    /// <summary>
    /// Storage of global configuration values, used by the whole SmartGlass library
    /// </summary>
    public static class GlobalConfiguration
    {
        /// <summary>
        /// Set bind address before any network interaction happens in case
        ///  binding to a specific interface/address is required
        /// 
        /// Defaults to IPAddress.Any
        /// </summary>
        /// <value>Local address to bind socket to</value>
        public static IPAddress BindAddress { get; set; } = IPAddress.Any;
    }
}