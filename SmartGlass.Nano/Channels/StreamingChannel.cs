using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class StreamingChannel
    {
        internal NanoClient _client;
        public ushort SequenceNumber { get; set; }
        public DateTime ReferenceTimestamp { get; private set; }
        public uint FrameId { get; private set; }
        internal bool IsOpen { get; set; }
        internal byte[] Flags { get; set; }

        public ushort NextSequenceNumber => ++SequenceNumber;
        public uint NextFrameId => ++FrameId;

        public abstract NanoChannel Channel { get; }

        public StreamingChannel()
        {
            IsOpen = false;
            SequenceNumber = 0;
        }

        public void RegisterOpen(NanoClient client, byte[] flags)
        {
            _client = client;
            Flags = flags;
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

        internal async Task SendChannelOpenAsync(NanoChannel channel)
        {
            var packet = new Nano.Packets.ChannelOpen(Flags);
            packet.Channel = channel;
            await _client.SendOnControlSocketAsync(packet);
        }

        internal async Task SendChannelCloseAsync(NanoChannel channel, uint reason)
        {
            var packet = new Nano.Packets.ChannelClose(reason);
            packet.Channel = Channel;
            await _client.SendOnControlSocketAsync(packet);
        }

        public async Task SendStreamerOnStreamingSocket(IStreamerMessage packet)
        {
            packet.Channel = Channel;
            packet.Header.SequenceNumber = NextSequenceNumber;
            packet.StreamerHeader.Flags = 0;

            await _client.SendOnStreamingSocketAsync(packet);
        }

        public async Task SendStreamerOnControlSocket(IStreamerMessage packet)
        {
            packet.Channel = Channel;
            packet.StreamerHeader.PreviousSequenceNumber = SequenceNumber;
            packet.StreamerHeader.SequenceNumber = NextSequenceNumber;
            packet.StreamerHeader.Flags = StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1;

            await _client.SendOnControlSocketAsync(packet);
        }
    }
}
