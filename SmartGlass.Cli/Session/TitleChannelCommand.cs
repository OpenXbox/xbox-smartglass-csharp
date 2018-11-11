using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Channels;
using NClap.Metadata;

namespace SmartGlass.Cli.Session
{
    internal class TitleChannelCommand : Command
    {
        // TODO: Move relay to the client library as a feature.

        private readonly object _remoteClientLock = new object();

        [PositionalArgument(ArgumentFlags.Required)]
        public uint TitleId { get; set; }

         [NamedArgument(ArgumentFlags.Optional)]
        public int? Port { get; set; }

        private void ReadFromSocket(TcpClient remoteClient, AuxiliaryStreamClient client)
        {
            Task.Run(async () =>
            {
                var buffer = new byte[remoteClient.ReceiveBufferSize];

                try
                {
                    while (remoteClient.Connected)
                    {
                        var length = remoteClient.Client.Receive(buffer);
                        await client.SendAsync(buffer.Take(length).ToArray());
                    }
                }
                catch (SocketException)
                {
                    // TODO: Tracing
                }

                Console.WriteLine($"Connection closed.");
            });
        }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            try
            {
                Console.WriteLine("Starting title channel...");
                using (var titleChannel = await ConnectCommand.Client.StartTitleChannelAsync(TitleId))
                {
                    Console.WriteLine("Title channel started.");

                    Console.WriteLine("Opening auxiliary stream...");
                    using (var client = await titleChannel.OpenAuxiliaryStreamAsync())
                    {
                        Console.WriteLine("Auxiliary stream opened.");

                        TcpClient remoteClient = null;

                        var remoteClientBacklog = new List<byte[]>();
                        client.DataReceived += (s, e) =>
                        {
                            lock (_remoteClientLock)
                            {
                                if (remoteClient == null)
                                {
                                    remoteClientBacklog.Add(e.Data);
                                }
                                else
                                {
                                    remoteClient.GetStream().Write(e.Data, 0, e.Data.Length);
                                }
                            }
                        };

                        var listener = new TcpListener(IPAddress.Any, Port ?? client.Port);
                        listener.ExclusiveAddressUse = true;
                        listener.Start();

                        Console.WriteLine($"Local TCP port {Port ?? client.Port} open. Waiting for connection...");
                        Console.WriteLine($"Press any key to exit.");

                        var remoteClientTask = listener.AcceptTcpClientAsync();

                        var relayConnectionTask = remoteClientTask.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                return;
                            }

                            remoteClient = t.Result;

                            Console.WriteLine($"Connection from {remoteClient.Client.RemoteEndPoint} accepted.");

                            ReadFromSocket(remoteClient, client);

                            lock (_remoteClientLock)
                            {
                                try
                                {
                                    foreach (var data in remoteClientBacklog)
                                    {
                                        remoteClient.GetStream().Write(data, 0, data.Length);
                                    }
                                }
                                catch
                                {
                                    // TODO: Trace
                                }
                            }
                        });

                        Console.ReadKey();

                        listener.Stop();

                        Console.WriteLine($"Title channel closed.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommandResult.RuntimeFailure;
            }

            return CommandResult.Success;
        }
    }
}