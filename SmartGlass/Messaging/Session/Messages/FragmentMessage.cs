using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    // Determined by IsFragment-flag in SessionFragmentMessageHeader
    // Data content depends on SessionMessageType 
    internal record FragmentMessage : SessionMessageBase
    {
        public uint SequenceBegin { get; set; }
        public uint SequenceEnd { get; set; }
        // Data: Plaintext SessionFragmentMessage payload
        public byte[] Data { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            SequenceBegin = reader.ReadUInt32BE();
            SequenceEnd = reader.ReadUInt32BE();
            Data = reader.ReadUInt16BEPrefixedBlob();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(SequenceBegin);
            writer.WriteBE(SequenceEnd);
            writer.WriteBE((ushort)Data.Length);
            writer.Write(Data);
        }
    }
}