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

        protected override void DeserializePayload(BEReader reader)
        {
            InitVector = reader.ReadBytes(16);
        }

        protected override void DeserializeProtectedPayload(BEReader reader)
        {
            Result = (ConnectionResult)reader.ReadUInt16();
            PairingState = (PairedIdentityState)reader.ReadUInt16();
            ParticipantId = reader.ReadUInt32();
        }

        protected override void SerializePayload(BEWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void SerializeProtectedPayload(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
