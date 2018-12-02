using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    // TODO: Allow client to specify values.
    [SessionMessageType(SessionMessageType.LocalJoin)]
    internal class LocalJoinMessage : SessionMessageBase
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

        public override void Deserialize(BEReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write((ushort)DeviceType);
            writer.Write(NativeWidth);
            writer.Write(NativeHeight);
            writer.Write(DpiX);
            writer.Write(DpiY);

            writer.Write((long)DeviceCapabilities);

            writer.Write(ClientVersion);
            writer.Write(OsMajorVersion);
            writer.Write(OsMinorVersion);

            writer.WriteUInt16Prefixed(DisplayName);
        }
    }
}