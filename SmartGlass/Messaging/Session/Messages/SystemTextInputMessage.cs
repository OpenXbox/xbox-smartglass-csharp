using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextInput)]
    internal class SystemTextInputMessage : SessionMessageBase
	{
        public uint TextSessionId { get; set; }
        public uint BaseVersion { get; set; }
        public uint SubmittedVersion { get; set; }
        public uint TotalTextBytelength { get; set; }
        public uint SelectionStart { get; set; }
        public uint SelectionLength { get; set; }
        public ushort Flags { get; set; }
        public uint TextChunkByteStart { get; set; }
        public string TextChunk { get; set; }
        public TextDelta[] Delta { get; set; }

        public SystemTextInputMessage()
		{
            Header.RequestAcknowledge = true;
		}

        public override void Deserialize(BEReader reader)
        {
            TextSessionId = reader.ReadUInt32();
            BaseVersion = reader.ReadUInt32();
            SubmittedVersion = reader.ReadUInt32();
            TotalTextBytelength = reader.ReadUInt32();
            SelectionStart = reader.ReadUInt32();
            SelectionLength = reader.ReadUInt32();
            Flags = reader.ReadUInt16();
            TextChunkByteStart = reader.ReadUInt32();
            TextChunk = reader.ReadString();
            Delta = reader.ReadArray<TextDelta>();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(TextSessionId);
            writer.Write(BaseVersion);
            writer.Write(SubmittedVersion);
            writer.Write(TotalTextBytelength);
            writer.Write(SelectionStart);
            writer.Write(SelectionLength);
            writer.Write(Flags);
            writer.Write(TextChunkByteStart);
            writer.Write(TextChunk);
            // writer.Write(Delta);
        }
	}
}