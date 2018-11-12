using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    internal abstract class StreamingChannelBase
    {
        internal readonly NanoClient _client;
        public NanoChannelId ChannelId { get; private set; }
        public ushort ChannelNumber { get; private set; }
        public bool IsOpen { get; private set; }
        public ushort SequenceNumber { get; set; }
        public DateTime ReferenceTimestamp { get; set; }
        public uint FrameId { get; set; }

        public ushort NextSequenceNumber
        {
            get
            {
                return ++SequenceNumber;
            }
        }

        public uint NextFrameId
        {
            get
            {
                return ++FrameId;
            }
        }

        public StreamingChannelBase(NanoClient client, NanoChannelId channelId)
        {
            _client = client;
            ChannelId = channelId;
            ChannelNumber = 0;
            SequenceNumber = 0;
            IsOpen = false;
        }

        public void OnChannelCreated(uint flags, ushort channelNumber)
        {
            ChannelNumber = channelNumber;
        }

        public void OnChannelOpened(byte[] flags)
        {
            if (IsOpen)
            {
                throw new Exception($"Channel {ChannelId} was already opened!");
            }
            SendChannelOpen(flags);
            IsOpen = true;
        }

        public void OnChannelClosed(uint flags)
        {
            if (!IsOpen)
            {
                throw new Exception($"Channel {ChannelId} is not open, can't be closed!");
            }
            SendChannelClose(flags);
            IsOpen = false;
        }

        private void SendChannelOpen(byte[] flags)
        {
            var payload = new Nano.Packets.ChannelControl(
                type: ChannelControlType.Open,
                data: new Nano.Packets.ChannelOpen(flags)
            );
            var packet = new RtpPacket(RtpPayloadType.ChannelControl, payload);
            SendOnControlSocket(packet);
        }

        private void SendChannelClose(uint flags)
        {
            var payload = new Nano.Packets.ChannelControl(
                type: ChannelControlType.Close,
                data: new Nano.Packets.ChannelClose(flags)
            );
            var packet = new RtpPacket(RtpPayloadType.ChannelControl, payload);
            SendOnControlSocket(packet);
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
            Debug.WriteLine("RefTimestamp for {0} set to: {1}", ChannelId, ReferenceTimestamp);
        }

        public uint GenerateInitialFrameId()
        {
            FrameId = (uint)new Random().Next(0, 500);
            return FrameId;
        }

        public void SendStreamerOnStreamingSocket(Streamer payload)
        {
            var packet = new RtpPacket(RtpPayloadType.Streamer, payload);

            packet.Header.SequenceNumber = NextSequenceNumber;
            ((Packets.Streamer)packet.Payload).Flags = 0;

            SendOnStreamingSocket(packet);
        }

        public void SendStreamerOnControlSocket(Streamer payload)
        {
            var packet = new RtpPacket(RtpPayloadType.Streamer, payload);

            ((Packets.Streamer)packet.Payload).PreviousSequenceNumber = SequenceNumber;
            ((Packets.Streamer)packet.Payload).SequenceNumber = NextSequenceNumber;
            ((Packets.Streamer)packet.Payload).Flags = StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1;

            packet.Header.ChannelId = ChannelNumber;
            SendOnControlSocket(packet);
        }

        private void SendOnStreamingSocket(RtpPacket packet)
        {
            packet.Header.ChannelId = ChannelNumber;
            _client.SendOnStreamingSocket(packet);
        }

        private void SendOnControlSocket(RtpPacket packet)
        {
            packet.Header.ChannelId = ChannelNumber;
            _client.SendOnControlSocket(packet);
        }
    }
}
