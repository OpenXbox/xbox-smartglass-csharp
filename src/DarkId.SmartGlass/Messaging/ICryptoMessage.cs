using DarkId.SmartGlass.Connection;

namespace DarkId.SmartGlass.Messaging
{
    public interface ICryptoMessage
    {
         CryptoContext Crypto { get; set; }
    }
}