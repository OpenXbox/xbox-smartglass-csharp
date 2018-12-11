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
        internal NanoRdpTransport _transport;

        /// <summary>
        /// Set to Datetime.Utc whenever _referenceTimestamp is set or (initially) got.
        /// Used to calculate local difference.
        /// Console uses bootup-DateTime as reference timestamp.
        /// This library uses DateTime.Now (UTC).
        /// </summary>
        /// <value></value>
        public DateTime OwnReferenceTimestamp { get; private set; }

        /// <summary>
        /// Set by ReferenceTimestamp property
        /// </summary>
        private DateTime _referenceTimestamp;

        /// <summary>
        /// Set by FrameId property
        /// </summary>
        private uint _frameId;
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Representation of reference timestamp DateTime as ulong
        /// </summary>
        /// <value></value>
        public ulong ReferenceTimestamp
        {
            get
            {
                // Only generate Timestamp on first call
                if (_referenceTimestamp == null)
                {
                    OwnReferenceTimestamp = DateTime.UtcNow;
                    _referenceTimestamp = OwnReferenceTimestamp;
                }

                return (ulong)(_referenceTimestamp - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            internal set
            {
                OwnReferenceTimestamp = DateTime.UtcNow;
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

        public event EventHandler<MessageReceivedEventArgs<INanoPacket>> MessageReceived;


        internal StreamingChannel(NanoRdpTransport transport, byte[] flags)
        {
            _transport = transport;
            _transport.MessageReceived += TransportMessageReceived;

            Flags = flags;
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

        public Task<INanoPacket> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return this.WaitForMessageAsync<INanoPacket, INanoPacket>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : INanoPacket
        {
            return this.WaitForMessageAsync<T, INanoPacket>(timeout, startAction, filter);
        }

        public void Dispose()
        {
            _transport.MessageReceived -= TransportMessageReceived;
        }
    }
}
