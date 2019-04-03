using System;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class ControlChannel : StreamingChannel, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Control;
        public override int ProtocolVersion => 0;

        internal ControlChannel(NanoRdpTransport transport, ChannelOpen openPacket)
            : base(transport, openPacket)
        {
            MessageReceived += OnMessage;
        }

        public async Task ChangeVideoQualityAsync(uint u1, uint u2, uint u3,
                                        uint u4, uint u5, uint u6)
        {
            var packet = new ChangeVideoQuality(u1, u2, u3, u4, u5, u6);
            await SendControlPacket(packet);
        }

        public async Task SendControllerEventAsync(ControllerEventType controllerType,
                                                    byte controllerIndex)
        {
            var packet = new ControllerEvent(controllerType, controllerIndex);
            await SendControlPacket(packet);
        }

        public void InitiateNetworkTest()
        {
        }

        private void OnRealtimeTelemetry(RealtimeTelemetry packet)
        {
        }

        private void OnNetworkTestResponse(NetworkTestResponse packet)
        {
        }

        public void OnMessage(object sender, MessageReceivedEventArgs<INanoPacket> args)
        {
            StreamerMessageWithHeader streamer = args.Message as StreamerMessageWithHeader;
            ControlOpCode opCode = streamer.ControlHeader.OpCode;
            switch (opCode)
            {
                case ControlOpCode.NetworkTestResponse:
                    OnNetworkTestResponse(streamer as NetworkTestResponse);
                    break;
                case ControlOpCode.RealtimeTelemetry:
                    OnRealtimeTelemetry(streamer as RealtimeTelemetry);
                    break;
                case ControlOpCode.InitiateNetworkTest:
                case ControlOpCode.NetworkInformation:
                case ControlOpCode.SessionCreate:
                case ControlOpCode.SessionCreateResponse:
                case ControlOpCode.SessionDestroy:
                case ControlOpCode.SessionInit:
                case ControlOpCode.VideoStatistics:
                case ControlOpCode.ChangeVideoQuality:
                case ControlOpCode.ControllerEvent:
                    throw new NotImplementedException(
                        $"Invalid control message received: {opCode}");
            }
        }

        private Task SendControlPacket(StreamerMessageWithHeader packet,
            ushort unknown1 = 1, ushort unknown2 = 1406)
        {
            packet.ControlHeader.Unknown1 = unknown1;
            packet.ControlHeader.Unknown2 = unknown2;
            packet.ControlHeader.PreviousSequence = SequenceNumber;

            return SendAsync(packet);
        }

        public async Task OpenAsync()
        {
            await SendChannelOpen(Channel, _channelOpenData.Flags);
        }
    }
}