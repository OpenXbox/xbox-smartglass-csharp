using SmartGlass.Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Auxiliary stream crypto context.
    /// </summary>
    internal class AuxiliaryStreamCryptoContext
    {
        private readonly byte[] _cryptoKey;
        private readonly byte[] _signHash;

        private readonly IBlockCipher _serverCipher;
        private readonly IBlockCipher _clientCipher;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.AuxiliaryStreamCryptoContext"/> class.
        /// </summary>
        /// <param name="cryptoKey">Crypto key.</param>
        /// <param name="serverInitVector">Server init vector.</param>
        /// <param name="clientInitVector">Client init vector.</param>
        /// <param name="signHash">Sign hash.</param>
        public AuxiliaryStreamCryptoContext(
            byte[] cryptoKey,
            byte[] serverInitVector,
            byte[] clientInitVector,
            byte[] signHash)
        {
            _cryptoKey = cryptoKey;
            _signHash = signHash;

            _serverCipher = CreateCipher(serverInitVector, false);
            _clientCipher = CreateCipher(clientInitVector, true);
        }

        /// <summary>
        /// Creates the cipher context
        /// </summary>
        /// <returns>The cipher.</returns>
        /// <param name="initVector">Init vector.</param>
        /// <param name="encrypt">If set to <c>true</c> encrypt.</param>
        private IBlockCipher CreateCipher(byte[] initVector, bool encrypt)
        {
            var aesCipher = new AesEngine();
            var blockCipher = new CbcBlockCipher(aesCipher);

            var keyParams = new KeyParameter(_cryptoKey);
            var paramsWithIv = new ParametersWithIV(keyParams, initVector);

            blockCipher.Init(encrypt, paramsWithIv);

            return blockCipher;
        }

        /// <summary>
        /// Encrypt the specified data.
        /// </summary>
        /// <returns>The encrypted data</returns>
        /// <param name="data">Encrypted data.</param>
        public byte[] Encrypt(byte[] data)
        {
            var writer = new BEWriter();
            byte[] padding = Padding.CreatePaddingData(
                PaddingType.PKCS7,
                data,
                alignment: 16);

            writer.Write(data);
            writer.Write(padding);

            var paddedData = writer.ToBytes();

            var output = new byte[paddedData.Length];

            for (var i = 0; i < paddedData.Length; i += 16)
            {
                _clientCipher.ProcessBlock(paddedData, i, output, i);
            }

            return output;
        }

        /// <summary>
        /// Decrypt the specified data.
        /// </summary>
        /// <returns>The decrypted data</returns>
        /// <param name="data">Decrypted data.</param>
        public byte[] Decrypt(byte[] data)
        {
            var output = new byte[data.Length];

            for (var i = 0; i < data.Length; i += 16)
            {
                _serverCipher.ProcessBlock(data, i, output, i);
            }

            return output;
        }

        /// <summary>
        /// Calculates the message signature (HMAC-SHA256)
        /// </summary>
        /// <returns>The message signature.</returns>
        /// <param name="bytes">Message signature as bytes.</param>
        public byte[] CalculateMessageSignature(byte[] bytes)
        {
            return MacUtilities.CalculateMac(
                "HMAC-SHA256",
                new KeyParameter(_signHash),
                bytes);
        }
    }
}
