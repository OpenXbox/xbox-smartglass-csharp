using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging
{
    interface IMessageHeader : ISerializable
    {
         MessageType Type { get; set; }
         ushort PayloadLength { get; set; }
    }
}