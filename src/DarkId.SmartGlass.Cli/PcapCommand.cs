using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using DarkId.SmartGlass.Analysis;
using NClap.Metadata;
using Tx.Network;

namespace DarkId.SmartGlass.Cli
{
    public class PcapCommand : SynchronousCommand
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

            var cryptoBlob = SharedSecret.Split(2).Select(c =>
                byte.Parse(new string(c.ToArray()), NumberStyles.HexNumber)).ToArray();

            var decryptor = new MessageAnalyzer(cryptoBlob);

            foreach (var datagram in datagrams)
            {
                var messageInfo = decryptor.ReadMessage(datagram.UdpData.ToArray());
                Console.WriteLine($"From: {datagram.UdpDatagramHeader.SourcePort}, To: {datagram.UdpDatagramHeader.DestinationPort}");
                Console.WriteLine($"Message Type: {messageInfo.MessageType}");
                Console.WriteLine($"Request Ack: {messageInfo.RequestAcknowledge}");
                Console.WriteLine($"Version: {messageInfo.Version}");
                Console.WriteLine($"ChannelId: {messageInfo.ChannelId}");
                Console.WriteLine($"Data: {BitConverter.ToString(messageInfo.Data).Replace("-", "").ToLower()}");
                Console.WriteLine();
            }

            return CommandResult.Success;
        }
    }
}