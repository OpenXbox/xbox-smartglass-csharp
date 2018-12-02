using System;
namespace SmartGlass.Nano
{
    public class NanoChannelClass
    {
        public const string Video = "Microsoft::Rdp::Dct::Channel::Class::Video";
        public const string Audio = "Microsoft::Rdp::Dct::Channel::Class::Audio";
        public const string ChatAudio = "Microsoft::Rdp::Dct::Channel::Class::ChatAudio";
        public const string Control = "Microsoft::Rdp::Dct::Channel::Class::Control";
        public const string Input = "Microsoft::Rdp::Dct::Channel::Class::Input";
        public const string InputFeedback = "Microsoft::Rdp::Dct::Channel::Class::Input Feedback";
        public const string TCPBase = "Microsoft::Rdp::Dct::Channel::Class::TcpBase";

        public static NanoChannel GetIdByClassName(string cls)
        {
            switch (cls)
            {
                case NanoChannelClass.Video: return NanoChannel.Video;
                case NanoChannelClass.Audio: return NanoChannel.Audio;
                case NanoChannelClass.ChatAudio: return NanoChannel.ChatAudio;
                case NanoChannelClass.Control: return NanoChannel.Control;
                case NanoChannelClass.Input: return NanoChannel.Input;
                case NanoChannelClass.InputFeedback: return NanoChannel.InputFeedback;
                case NanoChannelClass.TCPBase: return NanoChannel.TcpBase;
            }
            throw new NotSupportedException($"Unsupported ChannelClass {cls}");
        }
    }
}
