using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class ControlChannel : StreamingChannelBase, IStreamingChannel
    {
        public ControlChannel(NanoClient client)
            : base(client, NanoChannel.Control)
        {
        }

        public void ChangeVideoQuality(uint u1, uint u2, uint u3,
                                        uint u4, uint u5, uint u6)
        {
            var packet = new ChangeVideoQuality(u1, u2, u3, u4, u5, u6);
            SendControlPacket(packet);
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

        public void SendControlPacket(StreamerMessageWithHeader packet)
        {
            packet.ControlHeader.Unknown1 = 1;
            packet.ControlHeader.Unknown2 = 1406;
            SendStreamerOnControlSocket(packet);
        }
    }
}