using System;

namespace SmartGlass
{
    /// <summary>
    /// Public key type.
    /// Used in CryptoContext.
    /// </summary>
    public enum PublicKeyType : ushort
    {
        EC_DH_P256,
        EC_DH_P384,
        EC_DH_P521
    }
}
