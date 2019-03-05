using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace SmartGlass.Connection
{
    /// <summary>
    /// Certificate extensions.
    /// </summary>
    internal static class CertificateExtensions
    {
        /// <summary>
        /// Gets the LiveID from a X509 certificate.
        /// It assumes that the ID is contained in "SubjectDN" field.
        /// </summary>
        /// <returns>The LiveID.</returns>
        /// <param name="cert">Certificate received with DiscoveryResponse.</param>
        public static string GetLiveId(this X509Certificate cert)
        {
            var subjectDnValues = cert.SubjectDN.GetValueList();
            if (subjectDnValues.Count < 1)
                throw new InvalidDataException("Certificate does not contain SubjectDN");

            return (string)subjectDnValues[0];
        }
    }
}
