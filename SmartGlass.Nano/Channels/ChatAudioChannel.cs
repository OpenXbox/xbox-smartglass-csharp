using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class ChatAudioChannel : AudioChannelBase
    {
        public bool HandshakeDone { get; internal set; }
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }
        public Packets.AudioFormat ActiveFormat { get; internal set; }

        public ChatAudioChannel(NanoClient client)
            : base(client, NanoChannel.ChatAudio)
        {
            HandshakeDone = false;
        }

        public void OnChatAudioConfigReceived(object sender, AudioFormatEventArgs args)
        {
        }

        public void OnChatAudioDataReceived(object sender, AudioDataEventArgs args)
        {
        }

        public override void OnClientHandshake(AudioClientHandshake handshake)
        {
        }

        public override void OnServerHandshake(AudioServerHandshake handshake)
        {
            throw new NotSupportedException("ServerHandshake on client side");
        }

        public override void OnControl(AudioControl control)
        {
        }

        public override void OnData(AudioData data)
        {
            throw new NotSupportedException("ChatAudio data on client side");
        }
    }
}