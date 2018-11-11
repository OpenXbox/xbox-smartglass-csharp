namespace DarkId.SmartGlass.Messaging
{
    interface IProtectedMessageHeader : IMessageHeader
    {
         ushort ProtectedPayloadLength { get; set; }
    }
}