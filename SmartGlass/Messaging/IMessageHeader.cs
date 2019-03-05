using SmartGlass.Common;

namespace SmartGlass.Messaging
{
    /// <summary>
    /// Message header.
    /// </summary>
    interface IMessageHeader : ISerializable
    {
         MessageType Type { get; set; }
         ushort PayloadLength { get; set; }
    }
}
