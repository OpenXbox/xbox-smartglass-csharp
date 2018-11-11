using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DarkId.SmartGlass.Common
{
    internal static class SocketExtensions
    {
        public static CancellationTokenSource ConsumeReceived(this UdpClient client, Action<UdpReceiveResult> consume)
        {
            var tokenSource = new CancellationTokenSource();
            client.ConsumeReceived(consume, tokenSource.Token);
            return tokenSource;
        }

        public static void ConsumeReceived(
            this UdpClient client, Action<UdpReceiveResult> consume, CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested && client.Client != null)
                {
                    var receiveTask = client.ReceiveAsync();

                    Task.WaitAny(receiveTask);

                    if (!receiveTask.IsFaulted)
                    {
                        consume(receiveTask.Result);
                    }
                }
            });
        }

        public static CancellationTokenSource ConsumeReceived(this TcpClient client, Action<byte[]> consume)
        {
            var tokenSource = new CancellationTokenSource();
            client.ConsumeReceived(consume, tokenSource.Token);
            return tokenSource;
        }

        public static void ConsumeReceived(
            this TcpClient client, Action<byte[]> consume, CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested && client.Client != null)
                {
                    byte[] prefixBytes = new byte[sizeof(uint)];
                    NetworkStream stream = client.GetStream();
                    Task<int> bytesReadTask = stream.ReadAsync(prefixBytes, 0, sizeof(uint));

                    Task.WaitAny(bytesReadTask);

                    if (bytesReadTask.IsFaulted)
                    {
                        continue;
                    }

                    uint size = BitConverter.ToUInt32(prefixBytes, 0);
                    byte[] packet = new byte[size];

                    Task<int> receiveTask = stream.ReadAsync(packet, 0, packet.Length);

                    Task.WaitAny(receiveTask);

                    if (!receiveTask.IsFaulted)
                    {
                        consume(packet);
                    }
                }
            });
        }
    }
}