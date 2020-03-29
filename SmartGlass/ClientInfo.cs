using System;
using SmartGlass.Common;

namespace SmartGlass
{
    public class ClientInfo
    {
        public DeviceType DeviceType { get; set; }
        public ushort NativeWidth { get; set; }
        public ushort NativeHeight { get; set; }
        public ushort DpiX { get; set; }
        public ushort DpiY { get; set; }
        public DeviceCapabilities DeviceCapabilities { get; set; }
        public uint ClientVersion { get; set; }
        public uint OsMajorVersion { get; set; }
        public uint OsMinorVersion { get; set; }
        public string DisplayName { get; set; }

        public static ClientInfo GetDefaultWindowStoreClient()
        {
            return new ClientInfo()
            {
                DeviceType = DeviceType.WindowsStore,
                NativeWidth = 1280,
                NativeHeight = 720,
                DpiX = 96,
                DpiY = 96,
                DeviceCapabilities = DeviceCapabilities.SupportsAll,
                ClientVersion = 44,
                OsMajorVersion = 6,
                OsMinorVersion = 2,
                DisplayName = "SmartGlass"
            };
        }
    }
}