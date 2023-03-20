using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextInput)]
    internal record SystemTextInputMessage : SessionMessageBase
    {
        public uint TextSessionId { get; set; }
        public uint BaseVersion { get; set; }
        public uint SubmittedVersion { get; set; }
        public uint TotalTextBytelength { get; set; }
        public int SelectionStart { get; set; }
        public int SelectionLength { get; set; }
        public ushort Flags { get; set; }
        public uint TextChunkByteStart { get; set; }
        public string TextChunk { get; set; }
        public TextDelta[] Delta { get; set; }

        public SystemTextInputMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            TextSessionId = reader.ReadUInt32BE();
            BaseVersion = reader.ReadUInt32BE();
            SubmittedVersion = reader.ReadUInt32BE();
            TotalTextBytelength = reader.ReadUInt32BE();
            SelectionStart = reader.ReadInt32BE();
            SelectionLength = reader.ReadInt32BE();
            Flags = reader.ReadUInt16BE();
            TextChunkByteStart = reader.ReadUInt32BE();
            TextChunk = reader.ReadUInt16BEPrefixedString();
            if (reader.Length < reader.Position)//field is optional
                Delta = reader.ReadUInt16BEPrefixedArray<TextDelta>();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(TextSessionId);
            writer.WriteBE(BaseVersion);
            writer.WriteBE(SubmittedVersion);
            writer.WriteBE(TotalTextBytelength);
            writer.WriteBE(SelectionStart);
            writer.WriteBE(SelectionLength);
            writer.WriteBE(Flags);
            writer.WriteBE(TextChunkByteStart);
            writer.WriteUInt16BEPrefixed(TextChunk);
            //if (Delta != null)
                //writer.WriteUInt32BEPrefixedArray(Delta); //no be?
        }
    }
}