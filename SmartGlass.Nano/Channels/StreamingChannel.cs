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
