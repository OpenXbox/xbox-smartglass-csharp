using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using DarkId.SmartGlass.Analysis;
using NClap.Metadata;
using Tx.Network;

namespace DarkId.SmartGlass.Cli
{
    internal class PcapCommand : SynchronousCommand
    {
        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public string PcapFile { get; set; }

        [PositionalArgument(ArgumentFlags.Required, Position = 1)]
        public string SharedSecret { get; set; }

        [PositionalArgument(ArgumentFlags.Required, Position = 2)]
        public int Port { get; set; }

        public override CommandResult Execute()
        {
            var datagrams = Pcap.ReadFile(PcapFile).
                TrySelect(record =>
                    Tx.Network.PacketParser.Parse(record.Data.Skip(14).ToArray())).
                Where(p => p != null).
                Where(p => p.ProtocolType == ProtocolType.Udp).
                Select(p => p.ToUdpDatagram()).
                Where(p => p.UdpDatagramHeader.SourcePort == Port ||
                    p.UdpDatagramHeader.DestinationPort == Port).
                ToArray();

            var cryptoBlob = SharedSecret.HexToBytes();

            var decryptor = new MessageAnalyzer(cryptoBlob);

            foreach (var datagram in datagrams)
            {
                var messageInfo = decryptor.ReadMessage(datagram.UdpData.ToArray());
                Console.WriteLine($"From: {datagram.UdpDatagramHeader.SourcePort}, To: {datagram.UdpDatagramHeader.DestinationPort}");
                Console.WriteLine($"Message Type: {messageInfo.MessageType}");
                Console.WriteLine($"Request Ack: {messageInfo.RequestAcknowledge}");
                Console.WriteLine($"Version: {messageInfo.Version}");
                Console.WriteLine($"ChannelId: {messageInfo.ChannelId}");

                if (!String.IsNullOrEmpty(messageInfo.Json))
                {
                    Console.WriteLine($"Json: {messageInfo.Json}");
                }
                else
                {
                    Console.WriteLine($"Binary: {messageInfo.Data.ToHex()}");
                }

                Console.WriteLine();
            }

            return CommandResult.Success;
        }
    }
}