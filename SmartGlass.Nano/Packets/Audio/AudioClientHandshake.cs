using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.ClientHandshake)]
    internal class AudioClientHandshake : ISerializableLE
    {
        public uint InitialFrameID { get; private set; }
        public AudioFormat RequestedFormat { get; private set; }

        public AudioClientHandshake()
        {
            RequestedFormat = new AudioFormat();
        }
        
        public AudioClientHandshake(uint initialFrameID, AudioFormat requestedFormat)
        {
            InitialFrameID = initialFrameID;
            RequestedFormat = requestedFormat;
        }

        public void Deserialize(LEReader br)
        {
            InitialFrameID = br.ReadUInt32();
            ((ISerializableLE)RequestedFormat).Deserialize(br);
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(InitialFrameID);
            ((ISerializableLE)RequestedFormat).Serialize(bw);
        } 
    }
}
