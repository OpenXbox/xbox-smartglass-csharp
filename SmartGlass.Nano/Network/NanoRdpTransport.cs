using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano
{
    internal class NanoRdpTransport : IDisposable, IMessageTransport<RtpPacket>
    {
        private readonly TcpClient _controlProtoClient;
        private readonly UdpClient _streamingProtoClient;

        private readonly CancellationTokenSource _cancellationTokenSourceStreaming;
        private readonly CancellationTokenSource _cancellationTokenSourceControl;

        private readonly BlockingCollection<RtpPacket> _receiveQueue = new BlockingCollection<RtpPacket>();

        private readonly string _address;
        private readonly int _tcpPort;
        private readonly int _udpPort;

        private readonly IPEndPoint _controlProtoEp;
        private readonly IPEndPoint _streamingProtoEp;

        public event EventHandler<MessageReceivedEventArgs<RtpPacket>> MessageReceived;

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

            _cancellationTokenSourceControl = _controlProtoClient.ConsumeReceived(receiveResult =>
            {
                BEReader reader = new BEReader(receiveResult);
                RtpPacket packet = RtpPacket.CreateFromBuffer(reader);
                _receiveQueue.TryAdd(packet);
            });

            _cancellationTokenSourceStreaming = _streamingProtoClient.ConsumeReceived(receiveResult =>
            {
                // Lets NanoClient know when to end sending UDP handshake packets
                if (!udpDataActive)
                    udpDataActive = true;

                BEReader reader = new BEReader(receiveResult.Buffer);
                RtpPacket packet = RtpPacket.CreateFromBuffer(reader);
                _receiveQueue.TryAdd(packet);
            });

            Task.Run(() =>
            {
                while (!_receiveQueue.IsCompleted)
                {
                    try
                    {
                        var message = _receiveQueue.Take();
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs<RtpPacket>(message));
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
        public async Task SendAsync(RtpPacket message)
        {
            throw new InvalidOperationException("Please use SendAsyncStreaming/Control");
        }
#pragma warning restore 1998

        public async Task SendAsyncStreaming(RtpPacket message)
        {
            var writer = new BEWriter();
            message.Serialize(writer);
            var serialized = writer.ToArray();

            await _streamingProtoClient.SendAsync(serialized, serialized.Length);
        }

        public async Task SendAsyncControl(RtpPacket message)
        {
            var writer = new BEWriter();
            message.Serialize(writer);
            byte[] serialized = writer.ToArray();

            var finalWriter = new LEWriter();
            finalWriter.Write((uint)serialized.Length);
            finalWriter.Write(serialized);

            byte[] prefixedSerialized = finalWriter.ToArray();
            await _controlProtoClient.GetStream().WriteAsync(prefixedSerialized, 0, prefixedSerialized.Length);
        }

        public Task<RtpPacket> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return this.WaitForMessageAsync<RtpPacket, RtpPacket>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : RtpPacket
        {
            return this.WaitForMessageAsync<T, RtpPacket>(timeout, startAction, filter);
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
