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
    /// <summary>
    /// Crypto context.
    /// </summary>
    public class CryptoContext
    {
        private static readonly byte[] cryptoBlobPrepend =
            new byte[] { 0xd6, 0x37, 0xf1, 0xaa, 0xe2, 0xf0, 0x41, 0x8c };
        private static readonly byte[] cryptoBlobAppend =
            new byte[] { 0xa8, 0xf8, 0x1a, 0x57, 0x4e, 0x22, 0x8a, 0xb7 };

        /// <summary>
        /// Derives the key pair via elliptic curve algorithm.
        /// </summary>
        /// <returns>The generated key pair.</returns>
        /// <param name="certificate">Certificate containing console's pubkey.</param>
        private static AsymmetricCipherKeyPair GenerateKeyPair(X509Certificate certificate)
        {
            var consolePublicKey = (ECPublicKeyParameters)certificate.GetPublicKey();

            var gen = new ECKeyPairGenerator();
            gen.Init(new ECKeyGenerationParameters(consolePublicKey.Parameters, new SecureRandom()));

            return gen.GenerateKeyPair();
        }

        /// <summary>
        /// Generates the shared secret.
        /// </summary>
        /// <returns>The shared secret.</returns>
        /// <param name="clientPrivateKey">Client private key.</param>
        /// <param name="serverPublicKey">Server public key.</param>
        private static BigInteger GenerateSharedSecret(
            ICipherParameters clientPrivateKey,
            ICipherParameters serverPublicKey)
        {
            var agreement = AgreementUtilities.GetBasicAgreement("ECDH");
            agreement.Init(clientPrivateKey);

            return agreement.CalculateAgreement(serverPublicKey);
        }

        /// <summary>
        /// Creates the crypto BLOB.
        /// </summary>
        /// <returns>The crypto BLOB.</returns>
        /// <param name="sharedSecret">Shared secret.</param>
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

        /// <summary>
        /// Generates the random init vector.
        /// </summary>
        /// <returns>The random init vector.</returns>
        public static byte[] GenerateRandomInitVector()
        {
            return SecureRandom.GetNextBytes(new SecureRandom(), 16);
        }

        private readonly byte[] _publicKey;
        private byte[] _cryptoBlob;
        private byte[] _cryptoKey;
        private byte[] _derivationKey;
        private byte[] _hmacSecret;

        /// <summary>
        /// Gets the type of the public key.
        /// </summary>
        /// <value>The type of the public key.</value>
        public PublicKeyType PublicKeyType { get; private set; }
        /// <summary>
        /// Gets the public key as bytearray.
        /// </summary>
        /// <value>The public key.</value>
        public byte[] PublicKey => _publicKey;

        /// <summary>
        /// Gets the crypto BLOB.
        /// </summary>
        /// <value>The crypto BLOB.</value>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Connection.CryptoContext"/> class
        /// via X509 certificate.
        /// </summary>
        /// <param name="serverCertificate">Server certificate.</param>
        public CryptoContext(X509Certificate serverCertificate)
        {
            var keyPair = GenerateKeyPair(serverCertificate);
            PublicKeyType = ((ECPublicKeyParameters)keyPair.Public).ToPubKeyType();
            _publicKey = ((ECPublicKeyParameters)keyPair.Public).ToXYBlob();

            var sharedSecret = GenerateSharedSecret(keyPair.Private, serverCertificate.GetPublicKey());
            CryptoBlob = CreateCryptoBlob(sharedSecret);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Connection.CryptoContext"/> class
        /// via <paramref name="cryptoBlob"/>.
        /// </summary>
        /// <param name="cryptoBlob">Crypto BLOB.</param>
        /// <param name="publicKey">Public key.</param>
        public CryptoContext(byte[] cryptoBlob, byte[] publicKey = null)
        {
            CryptoBlob = cryptoBlob;
            PublicKeyType = PublicKeyType.EC_DH_P256;
            _publicKey = publicKey ?? new byte[64];
        }

        /// <summary>
        /// Creates the derived init vector.
        /// </summary>
        /// <returns>The derived init vector.</returns>
        /// <param name="data">Data.</param>
        public byte[] CreateDerivedInitVector(byte[] data)
        {
            var cipher = CipherUtilities.GetCipher(NistObjectIdentifiers.IdAes128Ecb);
            var keyParams = ParameterUtilities.CreateKeyParameter(NistObjectIdentifiers.IdAes128Ecb, _derivationKey);

            cipher.Init(true, keyParams);

            return cipher.DoFinal(data).Take(16).ToArray();
        }

        /// <summary>
        /// De/encrypt data via AES128-CBC
        /// </summary>
        /// <returns>The transformed data.</returns>
        /// <param name="data">Data to transform.</param>
        /// <param name="initVector">Init vector.</param>
        /// <param name="encrypt">If set to <c>true</c> data is encrypted, <c>false</c> decrypts.</param>
        private byte[] UseAesCipher(byte[] data, byte[] initVector, bool encrypt)
        {
            var cipher = CipherUtilities.GetCipher(NistObjectIdentifiers.IdAes128Cbc);
            var keyParams = ParameterUtilities.CreateKeyParameter(NistObjectIdentifiers.IdAes128Cbc, _cryptoKey);
            var paramsWithIv = new ParametersWithIV(keyParams, initVector);

            cipher.Init(encrypt, paramsWithIv);

            return cipher.DoFinal(data);
        }

        /// <summary>
        /// De/encrypt data via AES128-CBC without applying standarized
        /// padding (SmartGlass uses out-of-spec padding technique).
        /// </summary>
        /// <returns>The transformed data without padding.</returns>
        /// <param name="data">Data to transform.</param>
        /// <param name="initVector">Init vector.</param>
        /// <param name="encrypt">If set to <c>true</c> data is encrypted, <c>false</c> decrypts.</param>
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

        /// <summary>
        /// Encrypt the specified data.
        /// </summary>
        /// <returns>The encrypted data.</returns>
        /// <param name="data">Plaintext data.</param>
        /// <param name="initVector">Init vector.</param>
        public byte[] Encrypt(byte[] data, byte[] initVector)
        {
            return UseAesCipher(data, initVector, true);
        }

        /// <summary>
        /// Decrypt the specified data.
        /// </summary>
        /// <returns>The decrypted data.</returns>
        /// <param name="data">Plaintext data.</param>
        /// <param name="initVector">Init vector.</param>
        public byte[] Decrypt(byte[] data, byte[] initVector)
        {
            return UseAesCipher(data, initVector, false);
        }

        /// <summary>
        /// Encrypt the specified data without padding.
        /// </summary>
        /// <returns>The encrypted data.</returns>
        /// <param name="data">Plaintext data.</param>
        /// <param name="initVector">Init vector.</param>
        public byte[] EncryptWithoutPadding(byte[] data, byte[] initVector)
        {
            return UseAesCipherWithoutPadding(data, initVector, true);
        }

        /// <summary>
        /// Decrypt the specified data without padding.
        /// </summary>
        /// <returns>The decrypted data.</returns>
        /// <param name="data">Plaintext data.</param>
        /// <param name="initVector">Init vector.</param>
        public byte[] DecryptWithoutPadding(byte[] data, byte[] initVector)
        {
            return UseAesCipherWithoutPadding(data, initVector, false);
        }

        /// <summary>
        /// Calculates the message signature (HMAC-SHA256).
        /// </summary>
        /// <returns>The message signature.</returns>
        /// <param name="bytes">Signature bytes.</param>
        public byte[] CalculateMessageSignature(byte[] bytes)
        {
            return MacUtilities.CalculateMac(
                "HMAC-SHA256",
                new KeyParameter(_hmacSecret),
                bytes);
        }
    }
}
