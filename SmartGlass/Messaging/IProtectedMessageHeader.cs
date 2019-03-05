namespace SmartGlass.Messaging
{
    /// <summary>
    /// Protected message header.
    /// </summary>
    interface IProtectedMessageHeader : IMessageHeader
    {
         ushort ProtectedPayloadLength { get; set; }
    }
}
