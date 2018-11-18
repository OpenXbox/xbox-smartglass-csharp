using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    internal class NanoRdpTransport : IDisposable, IMessageTransport<INanoPacket>
    {
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
                    _receiveQueue.TryAdd(packet);
                }
                catch (NanoPackingException e)
                {
                    Debug.WriteLine($"Failed to parse nano packet: {e.Message}", e);
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
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs<INanoPacket>(message));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.WriteLine("Calling Nano MessageReceived failed!");
                    }
                }
            });
        }

#pragma warning disable 1998
        public async Task SendAsync(INanoPacket message)
        {
            throw new InvalidOperationException("Please use SendAsyncStreaming/Control");
        }
#pragma warning restore 1998

        public Task SendAsyncStreaming(INanoPacket message)
        {
            byte[] packet = NanoPacketFactory.AssemblePacket(message, ChannelContext);
            return _streamingProtoClient.SendAsync(packet, packet.Length);
        }

        public Task SendAsyncControl(INanoPacket message)
        {
            byte[] packet = NanoPacketFactory.AssemblePacket(message, ChannelContext);
            return _controlProtoClient.SendAsyncPrefixed(packet);
        }

        public void Dispose()
        {
            _receiveQueue.CompleteAdding();
            _cancellationTokenSourceStreaming.Cancel();
            _cancellationTokenSourceControl.Cancel();
            _streamingProtoClient.Dispose();
            _controlProtoClient.Dispose();
        }
    }
}
