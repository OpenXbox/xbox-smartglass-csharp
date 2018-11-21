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
        private DateTime _referenceTimestamp;
        private uint _frameId;
        public ushort SequenceNumber { get; private set; }

        public ulong ReferenceTimestamp
        {
            get
            {
                _referenceTimestamp = DateTime.UtcNow;
                return (ulong)(_referenceTimestamp - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            internal set
            {
                _referenceTimestamp = new DateTime(1970, 1, 1).AddMilliseconds(value).ToUniversalTime();
            }
        }

        public ulong Timestamp
        {
            get => (ulong)(DateTime.UtcNow - _referenceTimestamp).TotalMilliseconds;
        }

        public uint FrameId
        {
            get
            {
                if (_frameId != 0)
                    return _frameId;
                else
                    return _frameId = (uint)new Random().Next(0, 500);
            }
            internal set
            {
                _frameId = value;
            }
        }
        internal bool IsOpen { get; set; }
        internal byte[] Flags { get; set; }

        public ushort NextSequenceNumber => ++SequenceNumber;
        public uint NextFrameId => ++FrameId;

        public abstract NanoChannel Channel { get; }
        public abstract int ProtocolVersion { get; }

        public StreamingChannel()
        {
            SequenceNumber = 0;
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
