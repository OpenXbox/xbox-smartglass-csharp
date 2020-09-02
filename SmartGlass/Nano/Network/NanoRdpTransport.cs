using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    internal class NanoRdpTransport : IDisposable, IMessageTransport<INanoPacket>
    {
        private bool _disposed = false;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private static readonly ILogger logger = Logging.Factory.CreateLogger<NanoRdpTransport>();
        private readonly TcpClient _controlProtoClient;
        private readonly UdpClient _streamingProtoClient;

        private readonly BlockingCollection<INanoPacket> _receiveQueue = new BlockingCollection<INanoPacket>();

        private readonly string _address;
        private readonly int _tcpPort;
        private readonly int _udpPort;

        private readonly IPEndPoint _controlProtoEp;
        private readonly IPEndPoint _streamingProtoEp;

        internal ushort RemoteConnectionId { get; set; }
        public NanoChannelContext ChannelContext { get; private set; }
        public event EventHandler<MessageReceivedEventArgs<INanoPacket>> MessageReceived;

        public bool udpDataActive { get; private set; } = false;

        public NanoRdpTransport(string address, int tcpPort, int udpPort)
        {
            _address = address;
            _tcpPort = tcpPort;
            _udpPort = udpPort;

            _cancellationTokenSource = new CancellationTokenSource();
            IPAddress hostAddr = IPAddress.Parse(address);
            _controlProtoEp = new IPEndPoint(hostAddr, _tcpPort);
            _streamingProtoEp = new IPEndPoint(hostAddr, _udpPort);

            _controlProtoClient = new TcpClient(AddressFamily.InterNetwork);
            _streamingProtoClient = new UdpClient(AddressFamily.InterNetwork);

            _controlProtoClient.Client.Bind(new IPEndPoint(GlobalConfiguration.BindAddress, 0));
            _streamingProtoClient.Client.Bind(new IPEndPoint(GlobalConfiguration.BindAddress, 0));

            _controlProtoClient.Connect(_controlProtoEp);
            _streamingProtoClient.Connect(_streamingProtoEp);

            ChannelContext = new NanoChannelContext();

            void ProcessPacket(byte[] packetData)
            {
                try
                {
                    EndianReader reader = new EndianReader(packetData);
                    INanoPacket packet = NanoPacketFactory.ParsePacket(packetData, ChannelContext);

                    if (packet.Header.PayloadType == NanoPayloadType.ChannelControl &&
                        packet as ChannelCreate != null)
                    {
                        ChannelContext.RegisterChannel((ChannelCreate)packet);
                    }
                    else if (packet.Header.PayloadType == NanoPayloadType.ChannelControl &&
                             packet as ChannelClose != null)
                    {
                        ChannelContext.UnregisterChannel((ChannelClose)packet);
                    }
                    else if (RemoteConnectionId == 0 && packet as ControlHandshake != null)
                    {
                        RemoteConnectionId = ((ControlHandshake)packet).ConnectionId;
                    }

                    bool success = _receiveQueue.TryAdd(packet);
                    if (!success)
                        logger.LogTrace($"Failed to add message to receive queue");
                }
                catch (NanoPackingException e)
                {
                    logger.LogError($"Failed to parse nano packet: {e.Message}", e);
                }
            }

            _controlProtoClient.ConsumeReceivedPrefixed(
                receiveResult => ProcessPacket(receiveResult),
                _cancellationTokenSource.Token
            );

            _streamingProtoClient.ConsumeReceived(
                receiveResult => ProcessPacket(receiveResult.Buffer),
                _cancellationTokenSource.Token
            );

            Task.Run(() =>
            {
                while (!_receiveQueue.IsCompleted)
                {
                    try
                    {
                        var message = _receiveQueue.Take();
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs<INanoPacket>(message));
                    }
                    catch (Exception e)
                    {
                        logger.LogError(
                            e, "Calling Nano MessageReceived failed!");
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public Task SendAsync(INanoPacket message)
        {
            switch (message.Header.PayloadType)
            {
                case NanoPayloadType.ChannelControl:
                case NanoPayloadType.ControlHandshake:
                    return SendAsyncControl(message);
                case NanoPayloadType.UDPHandshake:
                    return SendAsyncStreaming(message);
                case NanoPayloadType.Streamer:
                    IStreamerMessage s = message as IStreamerMessage;
                    if (s.StreamerHeader.PacketType == 4)
                        return SendAsyncStreaming(message);
                    else
                        return SendAsyncControl(message);
                default:
                    throw new NanoException(
                        "SendAsync: Unexpected PayloadType: {message.Header.PayloadType}");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal Task SendChannelOpen(NanoChannel channel, byte[] flags)
        {
            var packet = new Nano.Packets.ChannelOpen(flags);
            packet.Channel = channel;
            return SendAsyncControl(packet);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        internal Task SendChannelClose(NanoChannel channel, uint reason)
        {
            var packet = new Nano.Packets.ChannelClose(reason);
            packet.Channel = channel;
            return SendAsyncControl(packet);
        }

        private Task SendAsyncStreaming(INanoPacket message)
        {
            if (RemoteConnectionId == 0)
            {
                throw new NanoException(
                    "ControlHandshake was not registered inside NanoChannelContext");
            }

            message.Header.ConnectionId = RemoteConnectionId;
            byte[] packet = NanoPacketFactory.AssemblePacket(message, ChannelContext);
            return _streamingProtoClient.SendAsync(packet, packet.Length);
        }

        private Task SendAsyncControl(INanoPacket message)
        {
            byte[] packet = NanoPacketFactory.AssemblePacket(message, ChannelContext);
            return _controlProtoClient.SendAsyncPrefixed(packet);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="startAction"></param>
        /// <returns></returns>
        internal Task<INanoPacket> WaitForMessageAsync(TimeSpan timeout, Func<Task> startAction = null)
        {
            return this.WaitForMessageAsync<INanoPacket, INanoPacket>(timeout, startAction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="startAction"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Func<Task> startAction = null, Func<T, bool> filter = null)
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
                    _receiveQueue.CompleteAdding();
                    _cancellationTokenSource.Cancel();
                    _streamingProtoClient.Dispose();
                    _controlProtoClient.Dispose();
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
