# Xbox One SmartGlass client library for .NET

[![Build Status](https://travis-ci.com/OpenXbox/xbox-smartglass-csharp.svg?branch=master)](https://travis-ci.com/OpenXbox/xbox-smartglass-csharp)
[![Build status](https://ci.appveyor.com/api/projects/status/3tgnu214x6qd87so?svg=true)](https://ci.appveyor.com/project/tuxuser/xbox-smartglass-csharp)
[![NuGet](https://img.shields.io/nuget/v/OpenXbox.SmartGlass.svg)](https://www.nuget.org/packages/OpenXbox.SmartGlass)
[![Discord](https://img.shields.io/badge/discord-OpenXbox-blue.svg)](https://discord.gg/E8kkJhQ)

## Build from source

```bash
cd xbox-smartglass-sharp
dotnet build
```

## Command line tool

Print usage

```bash
dotnet run -p SmartGlass.Cli -- --help
```

## API usage

### Discover consoles

```cs
using SmartGlass;

IEnumerable<SmartGlass.Device> devices = await SmartGlass.Device.DiscoverAsync();

foreach (SmartGlass.Device device in devices)
{
    Console.WriteLine($"{device.Name} ({device.HardwareId}) {device.Address}");
}
```

### Power on console

```cs
using SmartGlass;

string liveId = "FD0123456789";
SmartGlass.Device device = await SmartGlass.Device.PowerOnAsync(liveId);
Console.WriteLine($"{device.Name} ({device.HardwareId}) {device.Address}");
```

### Connect to console

```cs
using SmartGlass;

SmartGlassClient client = null;

try
{
    client = await SmartGlassClient.ConnectAsync("192.168.0.234");
}
catch (SmartGlassException e)
{
    Console.WriteLine($"Failed to connect: {e.Message}");
}
catch (TimeoutException)
{
    Console.WriteLine($"Timeout while connecting");
}
```

### Start Nano (gamestreaming)

```cs
using SmartGlass;
using SmartGlass.Channels;
using SmartGlass.Channels.Broadcast;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

SmartGlassClient client = /* Connect to console */;

// Get general gamestream configuration
GamestreamConfiguration config = GamestreamConfiguration.GetStandardConfig();
/* Modify standard config, if desired */

GamestreamSession session = await client.BroadcastChannel.StartGamestreamAsync(config);

Console.WriteLine(
    $"Connecting to NANO // TCP: {session.TcpPort}, UDP: {session.UdpPort}");

NanoClient nano = new NanoClient(hostName, session);

// General Handshaking & Opening channels
await nano.InitializeProtocolAsync();

// Audio & Video client handshaking
// Sets desired AV formats
AudioFormat audioFormat = nano.AudioFormats[0];
VideoFormat videoFormat = nano.VideoFormats[0];
await nano.InitializeStreamAsync(audioFormat, videoFormat);

// Start ChatAudio channel
AudioFormat chatAudioFormat = new AudioFormat(1, 24000, AudioCodec.Opus);
await nano.OpenChatAudioChannelAsync(chatAudioFormat);

IConsumer consumer = /* initialize consumer */;
nano.AddConsumer(consumer);

// Start consumer, if necessary
consumer.Start();

// Tell console to start sending AV frames
await nano.StartStreamAsync();

// Start Controller input channel
await nano.OpenInputChannelAsync(1280, 720);

/* Run a mainloop, to gather controller input events or similar */
```

---

Project originally developed by [Joel Day](https://github.com/joelday)

Special thanks to [Team OpenXbox](https://github.com/openxbox) for their
contribution of documentation, tools and samples for the SmartGlass protocol.
