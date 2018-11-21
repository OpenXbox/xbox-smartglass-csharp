using System;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Consumer;
using SmartGlass.Cli.Session;
using NClap.Metadata;
using NClap.Repl;

namespace SmartGlass.Cli
{
    internal class BroadcastCommand : Command
    {
        // TODO: https://github.com/reubeno/NClap/issues/30
        public static SmartGlassClient Client { get; private set; }

        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public string Hostname { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            Console.WriteLine($"Connecting to {Hostname}...");

            try
            {
                using (Client = await SmartGlassClient.ConnectAsync(Hostname))
                {
                    var broadcastChannel = Client.BroadcastChannel;
                    // TODO: Wait for BroadcastMessages here...

                    var config = GamestreamConfiguration.GetStandardConfig();
                    var result = await broadcastChannel.StartGamestreamAsync(config);
                    Console.WriteLine($"Connecting to Nano, TCP: {result.TcpPort}, UDP: {result.UdpPort}");
                    var nano = new NanoClient(Hostname, result);

                    Console.WriteLine($"Running protocol init...");
                    await nano.InitializeProtocolAsync();
                    await nano.OpenInputChannel(1280, 720);
                    await nano.OpenChatAudioChannel(
                        new Nano.Packets.AudioFormat(1, 24000, AudioCodec.Opus));

                    Console.WriteLine("Adding FileConsumer");
                    FileConsumer consumer = new FileConsumer("nanostream");
                    nano.AddConsumer(consumer);

                    Console.WriteLine("Initializing AV stream (handshaking)...");
                    await nano.InitializeStreamAsync(nano.Audio.AvailableFormats[0],
                                                nano.Video.AvailableFormats[0]);
                    Console.WriteLine("Starting stream...");
                    await nano.StartStreamAsync();
                    Console.WriteLine("Stream is running");

                    var loop = new Loop(typeof(SessionCommandType));
                    loop.Execute();

                    Console.WriteLine($"Disconnected");
                }

                return CommandResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to connect: {e}");
            }
            finally
            {
                Client = null;
            }

            return CommandResult.RuntimeFailure;
        }
    }
}
