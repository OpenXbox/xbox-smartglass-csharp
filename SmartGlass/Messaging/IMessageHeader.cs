using SmartGlass.Common;

namespace SmartGlass.Messaging
{
    interface IMessageHeader : ISerializable
    {
         MessageType Type { get; set; }
         ushort PayloadLength { get; set; }
    }
}