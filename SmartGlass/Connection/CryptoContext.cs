using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace SmartGlass.Connection
{
    public class CryptoContext
    {
        private static readonly byte[] cryptoBlobPrepend =
            new byte[] { 0xd6, 0x37, 0xf1, 0xaa, 0xe2, 0xf0, 0x41, 0x8c };
        private static readonly byte[] cryptoBlobAppend =
            new byte[] { 0xa8, 0xf8, 0x1a, 0x57, 0x4e, 0x22, 0x8a, 0xb7 };

        private static AsymmetricCipherKeyPair GenerateKeyPair(X509Certificate certificate)
        {
            var consolePublicKey = (ECPublicKeyParameters)certificate.GetPublicKey();

            var gen = new ECKeyPairGenerator();
            gen.Init(new ECKeyGenerationParameters(consolePublicKey.Parameters, new SecureRandom()));

            return gen.GenerateKeyPair();
        }

        private static BigInteger GenerateSharedSecret(
            ICipherParameters clientPrivateKey,
            ICipherParameters serverPublicKey)
        {
            var agreement = AgreementUtilities.GetBasicAgreement("ECDH");
            agreement.Init(clientPrivateKey);

            return agreement.CalculateAgreement(serverPublicKey);
        }

        private static byte[] CreateCryptoBlob(BigInteger sharedSecret)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(cryptoBlobPrepend);
                writer.Write(sharedSecret.ToByteArrayUnsigned());
                writer.Write(cryptoBlobAppend);

                var sha512 = SHA512.Create();
                sha512.Initialize();

                return sha512.ComputeHash(stream.ToArray());
            }
        }

        public static byte[] GenerateRandomInitVector()
        {
            return SecureRandom.GetNextBytes(new SecureRandom(), 16);
        }

        private readonly byte[] _publicKey;
        private byte[] _cryptoBlob;
        private byte[] _cryptoKey;
        private byte[] _derivationKey;
        private byte[] _hmacSecret;

        public byte[] PublicKey => _publicKey;

        public byte[] CryptoBlob
        {
            get
            {
                return _cryptoBlob;
            }

            private set
            {
                _cryptoBlob = value;

                _cryptoKey = _cryptoBlob.Take(16).ToArray();
                _derivationKey = _cryptoBlob.Skip(16).Take(16).ToArray();
                _hmacSecret = _cryptoBlob.Skip(32).Take(32).ToArray();
            }
        }

        public CryptoContext(X509Certificate serverCertificate)
        {
            var keyPair = GenerateKeyPair(serverCertificate);
            _publicKey = ((ECPublicKeyParameters)keyPair.Public).ToXYBlob();

            var sharedSecret = GenerateSharedSecret(keyPair.Private, serverCertificate.GetPublicKey());
            CryptoBlob = CreateCryptoBlob(sharedSecret);
        }

        public CryptoContext(byte[] cryptoBlob, byte[] publicKey = null)
        {
            CryptoBlob = cryptoBlob;
            _publicKey = publicKey ?? new byte[64];
        }

        public byte[] CreateDerivedInitVector(byte[] data)
        {
            var cipher = CipherUtilities.GetCipher(NistObjectIdentifiers.IdAes128Ecb);
            var keyParams = ParameterUtilities.CreateKeyParameter(NistObjectIdentifiers.IdAes128Ecb, _derivationKey);

            cipher.Init(true, keyParams); 

            return cipher.DoFinal(data).Take(16).ToArray();
        }

        private byte[] UseAesCipher(byte[] data, byte[] initVector, bool encrypt)
        {
            var cipher = CipherUtilities.GetCipher(NistObjectIdentifiers.IdAes128Cbc);
            var keyParams = ParameterUtilities.CreateKeyParameter(NistObjectIdentifiers.IdAes128Cbc, _cryptoKey);
            var paramsWithIv = new ParametersWithIV(keyParams, initVector);

            cipher.Init(encrypt, paramsWithIv);

            return cipher.DoFinal(data);
        }

        private byte[] UseAesCipherWithoutPadding(byte[] data, byte[] initVector, bool encrypt)
        {
            var aesCipher = new AesEngine();
            var blockCipher = new CbcBlockCipher(aesCipher);
            var cipher = new BufferedBlockCipher(blockCipher);

            var keyParams = new KeyParameter(_cryptoKey);
            var paramsWithIv = new ParametersWithIV(keyParams, initVector);

            cipher.Init(encrypt, paramsWithIv);

            return cipher.DoFinal(data);
        }

        public byte[] Encrypt(byte[] data, byte[] initVector)
        {
            return UseAesCipher(data, initVector, true);
        }

        public byte[] Decrypt(byte[] data, byte[] initVector)
        {
            return UseAesCipher(data, initVector, false);
        }

        public byte[] EncryptWithoutPadding(byte[] data, byte[] initVector)
        {
            return UseAesCipherWithoutPadding(data, initVector, true);
        }

        public byte[] DecryptWithoutPadding(byte[] data, byte[] initVector)
        {
            return UseAesCipherWithoutPadding(data, initVector, false);
        }

        public byte[] CalculateMessageSignature(byte[] bytes)
        {
            return MacUtilities.CalculateMac(
                "HMAC-SHA256",
                new KeyParameter(_hmacSecret),
                bytes);
        }
    }
}