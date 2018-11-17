using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class StreamingChannelBase
    {
        internal readonly NanoClient _client;
        public NanoChannel Channel { get; private set; }
        public ushort SequenceNumber { get; set; }
        public DateTime ReferenceTimestamp { get; private set; }
        public uint FrameId { get; private set; }
        public bool IsOpen { get; private set; }
        public bool HandshakeComplete { get; internal set; }

        public ushort NextSequenceNumber => ++SequenceNumber;
        public uint NextFrameId => ++FrameId;

        public StreamingChannelBase(NanoClient client, NanoChannel channel)
        {
            _client = client;
            Channel = channel;
            SequenceNumber = 0;
            IsOpen = false;
            HandshakeComplete = false;
        }

        public ulong GenerateTimestamp()
        {
            if (ReferenceTimestamp == null)
            {
                throw new InvalidOperationException("Reference Timestamp not set");
            }
            return (ulong)(DateTime.UtcNow - ReferenceTimestamp).TotalMilliseconds;
        }

        public ulong GenerateReferenceTimestamp()
        {
            ReferenceTimestamp = DateTime.UtcNow;
            return (ulong)(ReferenceTimestamp - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public void SetReferenceTimestamp(ulong timestamp)
        {
            ReferenceTimestamp = new DateTime(1970, 1, 1).AddMilliseconds(timestamp).ToUniversalTime();
            Debug.WriteLine("RefTimestamp for {0} set to: {1}", Channel, ReferenceTimestamp);
        }

        public uint GenerateInitialFrameId()
        {
            FrameId = (uint)new Random().Next(0, 500);
            return FrameId;
        }

        public void Create(uint flags)
        {
        }

        public void Open(byte[] flags)
        {
            IsOpen = true;
        }

        public void Close(uint flags)
        {
            IsOpen = false;
        }

        public void SendStreamerOnStreamingSocket(IStreamerMessage packet)
        {
            packet.Channel = Channel;
            packet.Header.SequenceNumber = NextSequenceNumber;
            packet.StreamerHeader.Flags = 0;

            _client.SendOnStreamingSocketAsync(packet).GetAwaiter().GetResult();
        }

        public void SendStreamerOnControlSocket(IStreamerMessage packet)
        {
            packet.Channel = Channel;
            packet.StreamerHeader.PreviousSequenceNumber = SequenceNumber;
            packet.StreamerHeader.SequenceNumber = NextSequenceNumber;
            packet.StreamerHeader.Flags = StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1;

            _client.SendOnControlSocketAsync(packet).GetAwaiter().GetResult();
        }
    }
}
