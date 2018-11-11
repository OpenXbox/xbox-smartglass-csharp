using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    internal class ControlChannel : StreamingChannelBase, IStreamingChannel
    {
        public ControlChannel(NanoClient client)
            : base(client, NanoChannelId.Control)
        {
        }

        public void ChangeVideoQuality()
        {
        }

        public void ControllerEvent()
        {
        }

        public void InitiateNetworkTest()
        {
        }

        public void OnStreamer(Streamer streamer)
        {
            streamer.DeserializeData(new StreamerMessageWithHeader());
            var message = streamer.Data as StreamerMessageWithHeader;

            switch(message.Header.OpCode)
            {
                case ControlOpCode.ChangeVideoQuality:
                case ControlOpCode.ControllerEvent:
                case ControlOpCode.InitiateNetworkTest:
                case ControlOpCode.NetworkInformation:
                case ControlOpCode.NetworkTestResponse:
                case ControlOpCode.RealtimeTelemetry:
                case ControlOpCode.SessionCreate:
                case ControlOpCode.SessionCreateResponse:
                case ControlOpCode.SessionDestroy:
                case ControlOpCode.SessionInit:
                case ControlOpCode.VideoStatistics:
                    break;
            }
        }
    }
}