using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace DarkId.SmartGlass.Connection
{
    internal static class CryptoExtensions
    {
        public static X509Certificate DeserializeCertificateAsn(byte[] bytes)
        {
            var asn = Asn1Object.FromByteArray(bytes);
            return new X509Certificate(X509CertificateStructure.GetInstance(asn));
        }

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