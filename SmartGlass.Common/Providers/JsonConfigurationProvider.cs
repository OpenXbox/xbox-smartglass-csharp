using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SmartGlass.Common.Enums;

namespace SmartGlass.Common.Providers
{
    public class JsonConfigurationProvider
    {
        IConfiguration Configuration;
        string[] localArgs;

        public JsonConfigurationProvider(string[] args, bool exceptions = false)
        {
            Configuration = GetConfiguration();

            if (args.Length <= 2) // this is not expected, try to use config!
            {
                var nargs = new List<string>();
                nargs.Add(args.Length > 0 ? args[0] : Configuration.GetValue("RunParameters:CommandName", "connect"));

                // adding help flag..
                if ((args.Length > 0 && args.Last().Contains("-h")))
                {
                    args = new string[] {
                        nargs.First(), // this is an assumption
                        "--help"
                    };
                }
                else if (args.Length == 0) // no params - whatever, for now.. clling help
                    args = new string[] { nameof(ParametersRunCommand.Help) };

                if (!args.Last().Contains("-h") || args.Length == 0) // new default behaviour (load json if parameters are unset)
                {
                    if (args.Length > 1)
                        nargs.Add(args[1] ?? Configuration.GetValue("RunParameters:HostName", "127.0.0.1"));
                    else
                        nargs.Add(Configuration.GetValue("RunParameters:HostName", "127.0.0.1"));

                    try
                    {
                        // this switch internally sets the default dommand to "connect"
                        switch ((ParametersRunCommand)System.Enum.Parse(typeof(ParametersRunCommand), nargs.First() ?? "connect", true))
                        {
                            // This switch should contin any run-type specific overrides passed to the commandline parser
                            case ParametersRunCommand.Pcap:
                                if (nargs[1] != null)
                                    nargs[1] = Configuration.GetValue("PcapParameters:CaptureFile", "./dump/raw_generated.pcap");
                                else
                                    nargs.Add(Configuration.GetValue("PcapParameters:CaptureFile", "./dump/raw_generated.pcap"));

                                // TODO: see comments behind
                                nargs.Add(Configuration.GetValue("PcapParameters:SharedSecret", "NO_SECRET_DEFINED")); // Eventually make this "dynamic"
                                nargs.Add(Configuration.GetValue("PcapParameters:Port", "5050")); // this is nearly useless..
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (exceptions)
                        {
                            throw new Exception($"ConfigruationProvider Error:\n{ex.Message}");
                        }
                        return; // Error! 
                    }
                    args = nargs.ToArray();
                }
            }
            // setting parsed args
            localArgs = args;
        }

        public string[] GetExtendedArgs()
        {
            return localArgs;
        }

        public IConfiguration GetConfiguration()
        {
            return Configuration ?? GetConfigurationFactory();
        }

        public static IConfiguration GetConfigurationFactory()
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            // Loading configuration
            ConfigurationBuilder confBuilder = new ConfigurationBuilder();
            return confBuilder.AddJsonFile($"{assemblyName}.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"{assemblyName}.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", optional: true)
                    .Build();
        }
    }
}