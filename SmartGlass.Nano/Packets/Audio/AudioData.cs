using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.Data)]
    public class AudioData : ISerializableLE
    {
        public uint Flags { get; private set; }
        public uint FrameId { get; private set; }
        public long Timestamp { get; private set; }
        public byte[] Data { get; private set; }

        public AudioData()
        {
        }
        
        public AudioData(uint flags, uint frameId,
                         long timestamp, byte[] data)
        {
            Flags = flags;
            FrameId = frameId;
            Timestamp = timestamp;
            Data = data;
        }

        void ISerializableLE.Deserialize(LEReader br)
        {
            Flags = br.ReadUInt32();
            FrameId = br.ReadUInt32();
            Timestamp = br.ReadInt64();
            Data = br.ReadBlobUInt32();
        }

        void ISerializableLE.Serialize(LEWriter bw)
        {
            bw.Write(Flags);
            bw.Write(FrameId);
            bw.Write(Timestamp);
            bw.Write((uint)Data.Length);
            bw.Write(Data);
        }
    }
}
