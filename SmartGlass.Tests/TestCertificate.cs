using System.Collections.Generic;
using SmartGlass.Common;
using SmartGlass.Connection;
using SmartGlass.Tests.Resources;
using Xunit;

namespace SmartGlass.Tests
{
    public class TestCertificate
    {
        public TestCertificate()
        {
        }

        [Fact]
        public void TestCertificateDeserialize()
        {
            byte[] cert = ResourcesProvider.GetBytes("selfsigned_cert.bin", ResourceType.Misc);
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);
            var publicKey = x509.GetPublicKey();

            Assert.NotNull(x509);
            Assert.Equal<int>(3, x509.Version);
            Assert.Equal("CN=Rust", x509.IssuerDN.ToString());
            Assert.Equal("CN=FFFFFFFFFFF", x509.SubjectDN.ToString());
            Assert.NotNull(publicKey);
        }
    }
}