using System;
using System.Diagnostics;
using AVFoundation;
using DarkId.SmartGlass.Nano.Consumer;
using DarkId.SmartGlass.Nano.Packets;
using Foundation;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public class AudioEngineManager : IDisposable
    {
        readonly AVAudioEngine _engine;
        readonly AVAudioPlayerNode _playerNode;
        readonly IAVFoundationAudioDataConsumer _consumer;


        public AudioEngineManager(AudioFormat format)
        {
            var avFormat = format.ToAVAudioFormat();

            _consumer = format.Codec == AudioCodec.PCM ?
                (IAVFoundationAudioDataConsumer)new PcmAudioBufferDataConsumer(avFormat) :
                new CompressedAudioBufferDataConsumer(avFormat); 

            _engine = new AVAudioEngine();
            _playerNode = new AVAudioPlayerNode();

            _engine.AttachNode(_playerNode);
            _engine.Connect(_playerNode, _engine.MainMixerNode, avFormat);

            _engine.StartAndReturnError(out NSError error);

            if (error != null)
            {
                Debug.WriteLine(error.ToString());
            }
        }

        public void ConsumeAudioData(AudioData data)
        {
            _consumer.ConsumeAudioData(data);

            // TODO: May need to interrupt buffer playback to make sure the audio doesn't drift behind?
            _playerNode.ScheduleBuffer(_consumer.Buffer, null, AVAudioPlayerNodeBufferOptions.InterruptsAtLoop, () =>
            {
                Debug.WriteLine("Buffer schedule completed.");
            });

            _playerNode.Play();
        }

        public void Dispose()
        {
            _engine.Dispose();
            _playerNode.Dispose();
            _consumer.Dispose();
        }
    }
}
