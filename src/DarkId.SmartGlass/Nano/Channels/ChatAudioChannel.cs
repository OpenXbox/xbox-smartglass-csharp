using System;
using DarkId.SmartGlass.Nano;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal class ChatAudioChannel : AudioChannelBase
    {
        public bool HandshakeDone { get; internal set; }
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }
        public Packets.AudioFormat ActiveFormat { get; internal set; }

        public ChatAudioChannel(NanoClient client)
            : base(client, NanoChannelId.ChatAudio)
        {
            HandshakeDone = false;
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