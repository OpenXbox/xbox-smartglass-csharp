using System.Collections.Generic;
using NUnit.Framework;
using SmartGlass.Common;
using SmartGlass.Connection;

namespace SmartGlass.Tests
{
    public class TestCertificate : TestDataProvider
    {
        public TestCertificate()
            : base("")
        {
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCertificateDeserialize()
        {
            byte[] cert = TestData["selfsigned_cert.bin"];
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);
            var publicKey = x509.GetPublicKey();

            Assert.IsNotNull(x509);
            Assert.AreEqual(3, x509.Version);
            Assert.AreEqual("CN=Rust", x509.IssuerDN.ToString());
            Assert.AreEqual("CN=FFFFFFFFFFF", x509.SubjectDN.ToString());
            Assert.IsNotNull(publicKey);
        }
    }
}