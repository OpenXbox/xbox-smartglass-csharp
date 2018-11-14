using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGlass.Common
{
    public static class SocketExtensions
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
            client.ConsumeReceivedPrefixed(consume, tokenSource.Token);
            return tokenSource;
        }

        public static void ConsumeReceivedPrefixed(
            this TcpClient client, Action<byte[]> consume, CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested && client.Client != null)
                {
                    NetworkStream ns = client.GetStream();
                    byte[] prefixBytes = new byte[sizeof(uint)];
                    Task<int> bytesReadTask = ns.ReadAsync(prefixBytes, 0, sizeof(uint));
                    Task.WaitAny(bytesReadTask);
                    if (bytesReadTask.IsFaulted)
                    {
                        Debug.WriteLine("ConsumeReceivedPrefixed: ReadAsync failed! (size prefix)");
                        continue;
                    }

                    uint size = BitConverter.ToUInt32(prefixBytes, 0);

                    byte[] packet = new byte[size];
                    Task<int> receiveTask = ns.ReadAsync(packet, 0, packet.Length);
                    Task.WaitAny(receiveTask);
                    if (receiveTask.IsFaulted)
                    {
                        Debug.WriteLine("ConsumeReceivedPrefixed: ReadAsync failed! (packet)");
                        continue;
                    }
                    consume(packet);
                }
            });
        }

        public static Task SendAsyncPrefixed(
            this TcpClient client, byte[] packet)
        {
            return Task.Run(() =>
            {
                NetworkStream ns = client.GetStream();
                byte[] lengthPrefix = BitConverter.GetBytes(packet.Length);

                BinaryWriter writer = new BinaryWriter(new MemoryStream());
                writer.Write(lengthPrefix);
                writer.Write(packet);
                byte[] prefixedData = writer.ToBytes();

                ns.Write(prefixedData, 0, prefixedData.Length);
            });
        }
    }
}