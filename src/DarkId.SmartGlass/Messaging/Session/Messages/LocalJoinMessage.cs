using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    // TODO: Allow client to specify values.
    [SessionMessageType(SessionMessageType.LocalJoin)]
    internal class LocalJoinMessage : SessionMessageBase
    {
        public DeviceType DeviceType { get; set; } = DeviceType.iPhone;
        public ushort NativeWidth { get; set; } = 640;
        public ushort NativeHeight { get; set; } = 1136;
        public ushort DpiX { get; set; } = 96;
        public ushort DpiY { get; set; } = 96;
        public DeviceCapabilities DeviceCapabilities { get; set; } = DeviceCapabilities.SupportsAll;
        public uint ClientVersion { get; set; } = 2;
        public uint OsMajorVersion = 5;
        public uint OsMinorVersion = 0;
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

            writer.Write(DisplayName);
        }
    }
}