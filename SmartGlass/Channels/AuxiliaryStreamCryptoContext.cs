using SmartGlass.Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SmartGlass.Channels
{
    internal class AuxiliaryStreamCryptoContext
    {
        private readonly byte[] _cryptoKey;
        private readonly byte[] _signHash;

        private readonly IBlockCipher _serverCipher;
        private readonly IBlockCipher _clientCipher;

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

        private IBlockCipher CreateCipher(byte[] initVector, bool encrypt)
        {
            var aesCipher = new AesEngine();
            var blockCipher = new CbcBlockCipher(aesCipher);

            var keyParams = new KeyParameter(_cryptoKey);
            var paramsWithIv = new ParametersWithIV(keyParams, initVector);

            blockCipher.Init(encrypt, paramsWithIv);

            return blockCipher;
        }

        public byte[] Encrypt(byte[] data)
        {
            var writer = new BEWriter();
            writer.WriteWithPaddingAlignment(data, 16);

            var padded = writer.ToArray();

            var output = new byte[padded.Length];

            for (var i = 0; i < padded.Length; i += 16)
            {
                _clientCipher.ProcessBlock(padded, i, output, i);
            }

            return output;
        }

        public byte[] Decrypt(byte[] data)
        {
            var output = new byte[data.Length];

            for (var i = 0; i < data.Length; i += 16)
            {
                _serverCipher.ProcessBlock(data, i, output, i);
            }

            return output;
        }

        public byte[] CalculateMessageSignature(byte[] bytes)
        {
            return MacUtilities.CalculateMac(
                "HMAC-SHA256",
                new KeyParameter(_signHash),
                bytes);
        }
    }
}