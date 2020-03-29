using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    // TODO: Allow client to specify values.
    [SessionMessageType(SessionMessageType.LocalJoin)]
    internal class LocalJoinMessage : SessionMessageBase
    {
        public ClientInfo ClientInfo { get; set; }

        public LocalJoinMessage(ClientInfo clientInfo)
        {
            ClientInfo = clientInfo;
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE((ushort)ClientInfo.DeviceType);
            writer.WriteBE(ClientInfo.NativeWidth);
            writer.WriteBE(ClientInfo.NativeHeight);
            writer.WriteBE(ClientInfo.DpiX);
            writer.WriteBE(ClientInfo.DpiY);

            writer.WriteBE((long)ClientInfo.DeviceCapabilities);

            writer.WriteBE(ClientInfo.ClientVersion);
            writer.WriteBE(ClientInfo.OsMajorVersion);
            writer.WriteBE(ClientInfo.OsMinorVersion);

            writer.WriteUInt16BEPrefixed(ClientInfo.DisplayName);
        }
    }
}