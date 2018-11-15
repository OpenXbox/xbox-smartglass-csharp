using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Connection
{
    [MessageType(MessageType.ConnectRequest)]
    internal class ConnectRequestMessage : ProtectedMessageBase<ConnectionMessageHeader>
    {
        public Guid DeviceId { get; set; }
        public byte[] PublicKey => Crypto.PublicKey;
        public string UserHash { get; set; }
        public string Authorization { get; set; }
        public uint SequenceNumber { get; set; }
        public uint SequenceBegin { get; set; }
        public uint SequenceEnd { get; set; }

        protected override void DeserializePayload(BEReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeProtectedPayload(BEReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void SerializePayload(BEWriter writer)
        {
            writer.Write(DeviceId.ToByteArray());

            writer.Write(new byte[2]);

            writer.Write(PublicKey);
            writer.Write(InitVector);
        }

        protected override void SerializeProtectedPayload(BEWriter writer)
        {
            writer.WriteUInt16Prefixed(UserHash);
            writer.WriteUInt16Prefixed(Authorization);

            writer.Write(SequenceNumber);
            writer.Write(SequenceBegin);
            writer.Write(SequenceEnd);
        }
    }
}