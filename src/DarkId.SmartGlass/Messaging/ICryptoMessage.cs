using DarkId.SmartGlass.Connection;

namespace DarkId.SmartGlass.Messaging
{
    interface ICryptoMessage
    {
         CryptoContext Crypto { get; set; }
    }
}