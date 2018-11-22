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
        private static readonly ILogger logger = Logging.Factory.CreateLogger<NanoRdpTransport>();
        private readonly TcpClient _controlProtoClient;
        private readonly UdpClient _streamingProtoClient;

        private readonly CancellationTokenSource _cancellationTokenSourceStreaming;
        private readonly CancellationTokenSource _cancellationTokenSourceControl;

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

            IPAddress hostAddr = IPAddress.Parse(address);
            _controlProtoEp = new IPEndPoint(hostAddr, _tcpPort);
            _streamingProtoEp = new IPEndPoint(hostAddr, _udpPort);

            _controlProtoClient = new TcpClient(AddressFamily.InterNetwork);
            _streamingProtoClient = new UdpClient(AddressFamily.InterNetwork);

            _controlProtoClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
            _streamingProtoClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));

            _controlProtoClient.Connect(_controlProtoEp);
            _streamingProtoClient.Connect(_streamingProtoEp);

            ChannelContext = new NanoChannelContext();

            void ProcessPacket(byte[] packetData)
            {
                try
                {
                    BEReader reader = new BEReader(packetData);
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

            _cancellationTokenSourceControl = _controlProtoClient.ConsumeReceived(
                receiveResult => ProcessPacket(receiveResult)
            );

            _cancellationTokenSourceStreaming = _streamingProtoClient.ConsumeReceived(
                receiveResult => ProcessPacket(receiveResult.Buffer)
            );

            Task.Run(() =>
            {
                while (!_receiveQueue.IsCompleted)
                {
                    try
                    {
                        var message = _receiveQueue.Take();
                        logger.LogTrace(
                            $"NANO: Received {message.Header.PayloadType} on Channel <{message.Channel}>");
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs<INanoPacket>(message));
                    }
                    catch (Exception e)
                    {
                        logger.LogError(
                            e, "Calling Nano MessageReceived failed!");
                    }
                }
            });
        }

        public async Task SendAsync(INanoPacket message)
        {
            logger.LogTrace(
                $"Sending {message.Header.PayloadType} on Channel <{message.Channel}>");

            switch (message.Header.PayloadType)
            {
                case NanoPayloadType.ChannelControl:
                case NanoPayloadType.ControlHandshake:
                    await SendAsyncControl(message);
                    break;
                case NanoPayloadType.UDPHandshake:
                    await SendAsyncStreaming(message);
                    break;
                case NanoPayloadType.Streamer:
                    IStreamerMessage s = message as IStreamerMessage;
                    if (s.StreamerHeader.PacketType == 4)
                        await SendAsyncStreaming(message);
                    else
                        await SendAsyncControl(message);
                    break;
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
        internal async Task SendChannelOpenAsync(NanoChannel channel, byte[] flags)
        {
            var packet = new Nano.Packets.ChannelOpen(flags);
            packet.Channel = channel;
            await SendAsyncControl(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        internal async Task SendChannelCloseAsync(NanoChannel channel, uint reason)
        {
            var packet = new Nano.Packets.ChannelClose(reason);
            packet.Channel = channel;
            await SendAsyncControl(packet);
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
        internal Task<INanoPacket> WaitForMessageAsync(TimeSpan timeout, Action startAction)
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
        internal Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : INanoPacket
        {
            return this.WaitForMessageAsync<T, INanoPacket>(timeout, startAction, filter);
        }

        public void Dispose()
        {
            _cancellationTokenSourceStreaming.Cancel();
            _cancellationTokenSourceControl.Cancel();
            _receiveQueue.CompleteAdding();
            _streamingProtoClient.Dispose();
            _controlProtoClient.Dispose();
        }
    }
}
