using System;
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
    }
}