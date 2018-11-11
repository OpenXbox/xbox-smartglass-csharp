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

        public static NanoChannelId GetIdByClassName(string cls)
        {
            switch(cls)
            {
                case NanoChannelClass.Video: return NanoChannelId.Video;
                case NanoChannelClass.Audio: return NanoChannelId.Audio;
                case NanoChannelClass.ChatAudio: return NanoChannelId.ChatAudio;
                case NanoChannelClass.Control: return NanoChannelId.Control;
                case NanoChannelClass.Input: return NanoChannelId.InputFeedback;
                case NanoChannelClass.TCPBase: return NanoChannelId.TcpBase;
                default:
                    return NanoChannelId.Unknown;
            }
        }
    }
}
