using System;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class ChatAudioChannel : AudioChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.ChatAudio;
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }
        public Packets.AudioFormat ActiveFormat { get; internal set; }

        public void OnChatAudioConfigReceived(object sender, AudioFormatEventArgs args)
        {
        }

        public void OnChatAudioDataReceived(object sender, AudioDataEventArgs args)
        {
        }

        public override void OnControl(AudioControl control)
        {
        }

        public override void OnData(AudioData data)
        {
            throw new NotSupportedException("ChatAudio data on client side");
        }

        public async Task OpenAsync()
        {
            await SendChannelOpenAsync(Channel);
        }
    }
}