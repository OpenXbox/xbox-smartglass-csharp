using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Connection
{
    /// <summary>
    /// Connect response message.
    /// Sent from console to client on successful connection.
    /// </summary>
    [MessageType(MessageType.ConnectResponse)]
    internal class ConnectResponseMessage : ProtectedMessageBase<ConnectionMessageHeader>
    {
        public ConnectionResult Result { get; set; }
        public PairedIdentityState PairingState { get; set; }
        public uint ParticipantId { get; set; }

        protected override void DeserializePayload(EndianReader reader)
        {
            InitVector = reader.ReadBytes(16);
        }

        protected override void DeserializeProtectedPayload(EndianReader reader)
        {
            Result = (ConnectionResult)reader.ReadUInt16BE();
            PairingState = (PairedIdentityState)reader.ReadUInt16BE();
            ParticipantId = reader.ReadUInt32BE();
        }

        protected override void SerializePayload(EndianWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void SerializeProtectedPayload(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
