using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace SmartGlass.Connection
{
    /// <summary>
    /// Crypto extensions.
    /// </summary>
    internal static class CryptoExtensions
    {
        /// <summary>
        /// Convert a ASN encoded certificate into X509Certificate object
        /// </summary>
        /// <returns>The certificate object.</returns>
        /// <param name="bytes">Certificate data.</param>
        public static X509Certificate DeserializeCertificateAsn(byte[] bytes)
        {
            var asn = Asn1Object.FromByteArray(bytes);
            return new X509Certificate(X509CertificateStructure.GetInstance(asn));
        }

        /// <summary>
        /// Gets the public key type from ECPublicKeyParameters
        /// </summary>
        /// <returns>The public key type.</returns>
        /// <param name="parameters">Public key parameters.</param>
        public static PublicKeyType ToPubKeyType(this ECPublicKeyParameters parameters)
        {
            switch (parameters.Parameters.Curve.FieldSize)
            {
                case 256:
                    return PublicKeyType.EC_DH_P256;
                case 384:
                    return PublicKeyType.EC_DH_P384;
                case 521:
                    return PublicKeyType.EC_DH_P521;
                default:
                    throw new InvalidDataException("Unsupported ECDH Curve");
            }
        }

        /// <summary>
        /// Converts ECPublicKeyParameters to XY blob.
        /// </summary>
        /// <returns>The XY blob.</returns>
        /// <param name="parameters">Public key parameters.</param>
        public static byte[] ToXYBlob(this ECPublicKeyParameters parameters)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(parameters.Q.XCoord.GetEncoded());
                writer.Write(parameters.Q.YCoord.GetEncoded());

                return stream.ToArray();
            }
        }
    }
}
