using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Packets
{
    public static class NanoPacketFactory
    {
        public static INanoPacket ParsePacket(byte[] data, NanoChannelContext context)
        {
            EndianReader packetReader = new EndianReader(data);
            RtpHeader header = new RtpHeader();
            header.Deserialize(packetReader);
            NanoPayloadType payloadType = header.PayloadType;

            // It might be NanoChannel.Unknown at this point, if ChannelCreate
            // packet was not processed yet
            // It gets processed at the end of the function
            NanoChannel channel = context.GetChannel(header.ChannelId);

            INanoPacket packet = null;
            long payloadOffset = packetReader.Position;

            switch (payloadType)
            {
                // FIXME: Create from Attribute is broken
                case NanoPayloadType.UDPHandshake:
                    packet = new UdpHandshake();
                    break;
                case NanoPayloadType.ControlHandshake:
                    packet = new ControlHandshake();
                    break;
                case NanoPayloadType.ChannelControl:
                    // Read type to pinpoint exact payload
                    ChannelControlType cct =
                        (ChannelControlType)packetReader.ReadUInt32LE();
                    packet = CreateFromChannelControlType(cct);
                    break;
                case NanoPayloadType.Streamer:
                    packet = CreateFromStreamerHeader(packetReader, channel);
                    break;
                default:
                    throw new NanoPackingException(
                        $"Unknown packet type received: {payloadType}");
            }

            if (packet == null)
            {
                throw new NanoPackingException("Failed to find matching body for packet");
            }

            packetReader.Seek(payloadOffset, SeekOrigin.Begin);
            packet.Deserialize(packetReader);
            packet.Header = header;

            if (packet as ChannelCreate != null)
            {
                string channelName = ((ChannelCreate)packet).Name;
                channel = NanoChannelClass.GetIdByClassName(channelName);
            }

            if (channel == NanoChannel.Unknown)
            {
                throw new NanoPackingException("ParsePacket: INanoPacket.Channel is UNKNOWN");
            }

            packet.Channel = channel;
            return packet;
        }

        public static byte[] AssemblePacket(INanoPacket packet, NanoChannelContext context)
        {
            if (packet.Channel == NanoChannel.Unknown)
            {
                throw new NanoPackingException("AssemblePacket: INanoPacket.Channel is UNKNOWN");
            }

            // Serialize payload and append padding if needed
            byte[] payloadData = null;
            using (EndianWriter payloadWriter = new EndianWriter())
            {
                packet.Serialize(payloadWriter);
                byte[] padding = Padding.CreatePaddingData(
                    PaddingType.ANSI_X923,
                    payloadWriter.ToBytes(),
                    alignment: 4);

                // Append padding
                if (padding.Length > 0)
                {
                    payloadWriter.Write(padding);
                    packet.Header.Padding = true;
                }

                payloadData = payloadWriter.ToBytes();
            }

            EndianWriter packetWriter = new EndianWriter();

            packet.Header.ChannelId = context.GetChannelId(packet.Channel);

            // Serialize header
            packet.Header.Serialize(packetWriter);
            // Append payload to header
            packetWriter.Write(payloadData);

            return packetWriter.ToBytes();
        }

        private static INanoPacket CreateFromStreamerHeader(EndianReader reader, NanoChannel channel)
        {
            if (channel == NanoChannel.Unknown)
            {
                throw new NanoPackingException(
                    $"Received Streamer Msg on UNKNOWN channel");
            }

            StreamerHeader streamerHeader = new StreamerHeader();
            streamerHeader.Deserialize(reader);
            uint streamerType = streamerHeader.PacketType;

            switch (channel)
            {
                case NanoChannel.Audio:
                case NanoChannel.ChatAudio:
                    return CreateFromAudioPayloadType((AudioPayloadType)streamerType);
                case NanoChannel.Video:
                    return CreateFromVideoPayloadType((VideoPayloadType)streamerType);
                case NanoChannel.Input:
                case NanoChannel.InputFeedback:
                    return CreateFromInputPayloadType((InputPayloadType)streamerType);
                case NanoChannel.Control:
                    // Skip to opCode
                    reader.Seek(8, SeekOrigin.Current);
                    ushort opCode = reader.ReadUInt16LE();
                    return CreateFromControlOpCode((ControlOpCode)opCode);
                default:
                    throw new NanoPackingException(
                        $"Received Streamer Msg on INVALID channel: {channel}");
            }
        }

        private static INanoPacket CreateFromChannelControlType(ChannelControlType controlType)
        {
            switch (controlType)
            {
                case ChannelControlType.Create:
                    return new ChannelCreate();
                case ChannelControlType.Open:
                    return new ChannelOpen();
                case ChannelControlType.Close:
                    return new ChannelClose();
                default:
                    throw new NanoPackingException($"Invalid ChannelControlType: {controlType}");
            }
        }

        private static INanoPacket CreateFromAudioPayloadType(AudioPayloadType audioType)
        {
            switch (audioType)
            {
                case AudioPayloadType.ClientHandshake:
                    return new AudioClientHandshake();
                case AudioPayloadType.ServerHandshake:
                    return new AudioServerHandshake();
                case AudioPayloadType.Control:
                    return new AudioControl();
                case AudioPayloadType.Data:
                    return new AudioData();
                default:
                    throw new NanoPackingException($"Invalid AudioPayloadType: {audioType}");
            }
        }

        private static INanoPacket CreateFromVideoPayloadType(VideoPayloadType videoType)
        {
            switch (videoType)
            {
                case VideoPayloadType.ClientHandshake:
                    return new VideoClientHandshake();
                case VideoPayloadType.ServerHandshake:
                    return new VideoServerHandshake();
                case VideoPayloadType.Control:
                    return new VideoControl();
                case VideoPayloadType.Data:
                    return new VideoData();
                default:
                    throw new NanoPackingException($"Invalid VideoPayloadType: {videoType}");
            }
        }

        private static INanoPacket CreateFromInputPayloadType(InputPayloadType inputType)
        {
            switch (inputType)
            {
                case InputPayloadType.ClientHandshake:
                    return new InputClientHandshake();
                case InputPayloadType.ServerHandshake:
                    return new InputServerHandshake();
                case InputPayloadType.Frame:
                    return new InputFrame();
                case InputPayloadType.FrameAck:
                    return new InputFrameAck();
                default:
                    throw new NanoPackingException($"Invalid InputPayloadType: {inputType}");
            }
        }

        private static INanoPacket CreateFromControlOpCode(ControlOpCode opCode)
        {
            switch (opCode)
            {
                case ControlOpCode.ChangeVideoQuality:
                    return new ChangeVideoQuality();
                case ControlOpCode.ControllerEvent:
                    return new ControllerEvent();
                case ControlOpCode.InitiateNetworkTest:
                    return new InitiateNetworkTest();
                case ControlOpCode.NetworkInformation:
                    return new NetworkInformation();
                case ControlOpCode.NetworkTestResponse:
                    return new NetworkTestResponse();
                case ControlOpCode.RealtimeTelemetry:
                    return new RealtimeTelemetry();
                case ControlOpCode.SessionCreate:
                    return new SessionCreate();
                case ControlOpCode.SessionCreateResponse:
                    return new SessionCreateResponse();
                case ControlOpCode.SessionDestroy:
                    return new SessionDestroy();
                case ControlOpCode.SessionInit:
                    return new SessionInit();
                case ControlOpCode.VideoStatistics:
                    return new VideoStatistics();
                default:
                    throw new NanoPackingException($"Invalid Control OpCode: {opCode}");
            }
        }
    }
}
