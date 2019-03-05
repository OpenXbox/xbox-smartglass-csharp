using SmartGlass.Connection;

namespace SmartGlass.Messaging
{
    /// <summary>
    /// Crypto message.
    /// </summary>
    interface ICryptoMessage
    {
         CryptoContext Crypto { get; set; }
    }
}
