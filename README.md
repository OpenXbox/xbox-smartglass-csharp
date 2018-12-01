# SmartGlass

[![Build status](https://ci.appveyor.com/api/projects/status/3tgnu214x6qd87so?svg=true)](https://ci.appveyor.com/project/tuxuser/xbox-smartglass-csharp)
[![NuGet](https://img.shields.io/nuget/v/OpenXbox.SmartGlass.svg)](https://www.nuget.org/packages/OpenXbox.SmartGlass)
[![Discord](https://img.shields.io/badge/discord-OpenXbox-blue.svg)](https://discord.gg/E8kkJhQ)

## Xbox One SmartGlass client library for .NET

---

Project originally developed by [Joel Day](https://github.com/joelday)
Special thanks to [Team OpenXbox](https://github.com/openxbox) for their
contribution of documentation, tools and samples for the SmartGlass protocol.

### CLI - Configuration

Before starting local testing in a "development" environment, make sure to override the default settings by doing the following:

```Bash
cp SmartGlass.Cli/SmartGlass.Cli.json SmartGlass.Cli/SmartGlass.Cli.Development.json
```

Local settings can now be made at "SmartGlass.Cli/SmartGlass.Cli.Development.json" (CommandName can be overridden!).
