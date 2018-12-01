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

            GamestreamSession session = null;
            SmartGlassClient Client = null;

            try
            {
                Client = await SmartGlassClient.ConnectAsync(Hostname);
            }
            catch (SmartGlassException e)
            {
                Console.WriteLine($"Failed to connect: {e.Message}");
                return CommandResult.RuntimeFailure;
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"Timeout while connecting");
                return CommandResult.RuntimeFailure;
            }

            var broadcastChannel = Client.BroadcastChannel;

            var config = GamestreamConfiguration.GetStandardConfig();

            try
            {
                session = await broadcastChannel.StartGamestreamAsync(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to send StartGamestream: {e.Message}");
                return CommandResult.RuntimeFailure;
            }

            Console.WriteLine($"Connecting to Nano, TCP: {session.TcpPort}, UDP: {session.UdpPort}");
            var nano = new NanoClient(Hostname, session);

            try
            {
                Console.WriteLine($"Running protocol init...");
                await nano.InitializeProtocolAsync();
                await nano.OpenInputChannel(1280, 720);
                await nano.OpenChatAudioChannel(
                    new Nano.Packets.AudioFormat(1, 24000, AudioCodec.Opus));

                Console.WriteLine("Adding FileConsumer");
                FileConsumer consumer = new FileConsumer("nanostream");
                nano.AddConsumer(consumer);

                Console.WriteLine("Initializing AV stream (handshaking)...");
                await nano.InitializeStreamAsync(nano.AudioFormats[0],
                                                 nano.VideoFormats[0]);
                Console.WriteLine("Starting stream...");
                await nano.StartStreamAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to initialize gamestream: {e}");
                return CommandResult.RuntimeFailure;
            }

            Console.WriteLine("Stream is running");

            var loop = new Loop(typeof(SessionCommandType));
            loop.Execute();

            return CommandResult.Success;
        }
    }
}
