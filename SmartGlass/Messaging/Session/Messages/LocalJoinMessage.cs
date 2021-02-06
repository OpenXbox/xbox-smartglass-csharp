using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    // TODO: Allow client to specify values.
    [SessionMessageType(SessionMessageType.LocalJoin)]
    internal record LocalJoinMessage : SessionMessageBase
    {
        public DeviceType DeviceType { get; set; } = DeviceType.WindowsStore;
        public ushort NativeWidth { get; set; } = 1280;
        public ushort NativeHeight { get; set; } = 720;
        public ushort DpiX { get; set; } = 96;
        public ushort DpiY { get; set; } = 96;
        public DeviceCapabilities DeviceCapabilities { get; set; } = DeviceCapabilities.SupportsAll;
        public uint ClientVersion { get; set; } = 44;
        public uint OsMajorVersion = 6;
        public uint OsMinorVersion = 2;
        public string DisplayName = "SmartGlass";

        public LocalJoinMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE((ushort)DeviceType);
            writer.WriteBE(NativeWidth);
            writer.WriteBE(NativeHeight);
            writer.WriteBE(DpiX);
            writer.WriteBE(DpiY);

            writer.WriteBE((long)DeviceCapabilities);

            writer.WriteBE(ClientVersion);
            writer.WriteBE(OsMajorVersion);
            writer.WriteBE(OsMinorVersion);

            writer.WriteUInt16BEPrefixed(DisplayName);
        }
    }
}