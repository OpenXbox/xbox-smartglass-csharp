using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class StreamingChannel : IMessageTransport<INanoPacket>, IDisposable
    {
        bool _disposed = false;
        readonly NanoRdpTransport _transport;

        /// <summary>
        /// Set by FrameId property
        /// </summary>
        private uint _frameId;
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Set by ReferenceTimestampUlong property.
        /// </summary>
        public DateTime ReferenceTimestamp { get; private set; }

        /// <summary>
        /// Representation of reference timestamp DateTime as ulong.
        /// Reference Timestamp is given in following format:
        /// - Milliseconds since Unix Epoch (01.01.1970 00:00:00)
        /// </summary>
        /// <value></value>
        public ulong ReferenceTimestampUlong
        {
            get
            {
                // Only generate Timestamp on first call
                if (ReferenceTimestamp.Ticks == 0)
                {
                    ReferenceTimestamp = DateTime.Now;
                }
                return DateTimeHelper.ToEpochMilliseconds(ReferenceTimestamp);
            }
            internal set
            {
                ReferenceTimestamp = DateTimeHelper.FromEpochMilliseconds(value);
            }
        }

        /// <summary>
        /// Timestamp used by Streamer messages.
        /// Format: Microseconds elapsed since reference timestamp.
        /// </summary>
        /// <value></value>
        public ulong Timestamp
        {
            get => DateTimeHelper.ToTimestampMicroseconds(DateTime.Now, ReferenceTimestamp);
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

        internal bool _isOpen { get; set; }
        internal ChannelOpen _channelOpenData { get; set; }

        public ushort NextSequenceNumber => ++SequenceNumber;
        public uint NextFrameId => ++FrameId;

        public abstract NanoChannel Channel { get; }
        public abstract int ProtocolVersion { get; }

        public event EventHandler<MessageReceivedEventArgs<INanoPacket>> MessageReceived;


        internal StreamingChannel(NanoRdpTransport transport, ChannelOpen openPacket)
        {
            _transport = transport;
            _transport.MessageReceived += TransportMessageReceived;

            _channelOpenData = openPacket;
            SequenceNumber = 0;
        }

        private void TransportMessageReceived(object sender, MessageReceivedEventArgs<INanoPacket> e)
        {
            if (e.Message.Channel == Channel)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        public Task SendAsync(INanoPacket packet)
        {
            var message = packet as IStreamerMessage;
            if (message.StreamerHeader.PacketType == 4)
            {
                // Data packet -> UDP
                message.Channel = Channel;
                message.Header.SequenceNumber = NextSequenceNumber;
                message.StreamerHeader.Flags = 0;

                return _transport.SendAsync(message);
            }
            else
            {
                // TCP
                message.Channel = Channel;
                message.StreamerHeader.PreviousSequenceNumber = SequenceNumber;
                message.StreamerHeader.SequenceNumber = NextSequenceNumber;
                message.StreamerHeader.Flags = StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1;

                return _transport.SendAsync(message);
            }
        }

        internal Task SendChannelOpen(NanoChannel channel, byte[] flags)
            => _transport.SendChannelOpen(channel, flags);

        public Task<INanoPacket> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return this.WaitForMessageAsync<INanoPacket, INanoPacket>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : INanoPacket
        {
            return this.WaitForMessageAsync<T, INanoPacket>(timeout, startAction, filter);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transport.MessageReceived -= TransportMessageReceived;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
