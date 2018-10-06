using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    // Determined by IsFragment-flag in SessionFragmentMessageHeader
    // Data content depends on SessionMessageType 
    internal class FragmentMessage : SessionMessageBase
    {
        public uint SequenceBegin { get; set; }
        public uint SequenceEnd { get; set; }
        // Data: Plaintext SessionFragmentMessage payload
        public byte[] Data { get; set; }

        public override void Deserialize(BEReader reader)
        {
            SequenceBegin = reader.ReadUInt32();
            SequenceEnd = reader.ReadUInt32();
            Data = reader.ReadBlob();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(SequenceBegin);
            writer.Write(SequenceEnd);
            writer.Write((ushort)Data.Length);
            writer.Write(Data);
        }
    }
}