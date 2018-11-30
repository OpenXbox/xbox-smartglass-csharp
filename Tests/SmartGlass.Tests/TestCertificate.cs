using System.Collections.Generic;
using SmartGlass.Common;
using SmartGlass.Connection;
using Xunit;

namespace SmartGlass.Tests
{
    public class TestCertificate : TestDataProvider
    {
        public TestCertificate()
            : base("")
        {
        }

        [Fact]
        public void TestCertificateDeserialize()
        {
            byte[] cert = TestData["selfsigned_cert.bin"];
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);
            var publicKey = x509.GetPublicKey();

            Assert.NotNull(x509);
            Assert.Equal<int>(3, x509.Version);
            Assert.Equal<string>("CN=Rust", x509.IssuerDN.ToString());
            Assert.Equal<string>("CN=FFFFFFFFFFF", x509.SubjectDN.ToString());
            Assert.NotNull(publicKey);
        }
    }
}