using System;
using System.Collections.Generic;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal class ChannelManager
    {
        private NanoClient _client;
        private Dictionary<ushort,NanoChannelId> _channels; 

        public AudioChannel Audio { get; private set; }
        public ChatAudioChannel ChatAudio { get; private set; }
        public ControlChannel Control { get; private set; }
        public InputChannel Input { get; private set; }
        public InputFeedbackChannel InputFeedback { get; private set; }
        public VideoChannel Video { get; private set; }

        public ChannelManager(NanoClient client)
        {
            _client = client;
            _channels = new Dictionary<ushort, NanoChannelId>();
            Audio = new AudioChannel(_client);
            ChatAudio = new ChatAudioChannel(_client);
            Control = new ControlChannel(_client);
            Input = new InputChannel(_client);
            InputFeedback = new InputFeedbackChannel(_client);
            Video = new VideoChannel(_client);
        }

        public StreamingChannelBase GetChannelById(NanoChannelId id)
        {
            switch(id)
            {
                case NanoChannelId.Audio: return Audio;
                case NanoChannelId.ChatAudio: return ChatAudio;
                case NanoChannelId.Control: return Control;
                case NanoChannelId.Input: return Input;
                case NanoChannelId.InputFeedback: return InputFeedback;
                case NanoChannelId.Video: return Video;
                default:
                    throw new NotSupportedException($"Unsupported NanoChannelId: {id}");
            }
        }

        public NanoChannelId GetChannel(ushort channelNumber)
        {
            NanoChannelId id;
            if (!_channels.TryGetValue(channelNumber, out id))
            {
                throw new InvalidOperationException($"No channel found for ChannelNumber {channelNumber}");
            }
            return id;
        }

        private void HandleChannelCreate(Packets.ChannelCreate payload, ushort channelNumber)
        {
            NanoChannelId id = NanoChannelClass.GetIdByClassName(payload.Name);
            _channels[channelNumber] = id;
            GetChannelById(id).OnChannelCreated(payload.Flags, channelNumber);
        }

        private void HandleChannelOpen(Packets.ChannelOpen payload, ushort channelNumber)
        {
            NanoChannelId id = GetChannel(channelNumber);
            GetChannelById(id).OnChannelOpened(payload.Flags);
        }

        private void HandleChannelClose(Packets.ChannelClose payload, ushort channelNumber)
        {
            NanoChannelId id = GetChannel(channelNumber);
            GetChannelById(id).OnChannelClosed(payload.Flags);
        }

        public void HandleChannelControl(Packets.ChannelControl controlMsg, ushort channelNumber)
        {
            switch(controlMsg.Type)
            {
                case ChannelControlType.Create:
                    var create = controlMsg.Data as Nano.Packets.ChannelCreate;
                    HandleChannelCreate(create, channelNumber);
                    break;
                case ChannelControlType.Open:
                    var open = controlMsg.Data as Nano.Packets.ChannelOpen;
                    HandleChannelOpen(open, channelNumber);
                    break;
                case ChannelControlType.Close:
                    var close = controlMsg.Data as Nano.Packets.ChannelClose;
                    HandleChannelClose(close, channelNumber);
                    break;
            }
        }

        public void HandleStreamer(Packets.Streamer streamer, ushort channelNumber)
        {
            NanoChannelId id = GetChannel(channelNumber);
            ((IStreamingChannel)GetChannelById(id)).OnStreamer(streamer);
        }
    }
}