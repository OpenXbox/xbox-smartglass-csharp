using System;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class ControlChannel : StreamingChannel, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Control;

        public async Task ChangeVideoQuality(uint u1, uint u2, uint u3,
                                        uint u4, uint u5, uint u6)
        {
            var packet = new ChangeVideoQuality(u1, u2, u3, u4, u5, u6);
            await SendControlPacketAsync(packet);
        }

        public void ControllerEvent()
        {
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

        public void OnPacket(IStreamerMessage packet)
        {
            StreamerMessageWithHeader streamer = packet as StreamerMessageWithHeader;
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

        public async Task SendControlPacketAsync(StreamerMessageWithHeader packet)
        {
            packet.ControlHeader.Unknown1 = 1;
            packet.ControlHeader.Unknown2 = 1406;
            await SendStreamerOnControlSocket(packet);
        }

        public async Task OpenAsync()
        {
            await SendChannelOpenAsync(Channel);
        }
    }
}