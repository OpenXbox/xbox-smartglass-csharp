using SmartGlass.Connection;

namespace SmartGlass.Messaging
{
    interface ICryptoMessage
    {
         CryptoContext Crypto { get; set; }
    }
}