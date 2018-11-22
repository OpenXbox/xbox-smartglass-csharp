using System.Collections.Generic;
using NUnit.Framework;
using SmartGlass.Common;
using SmartGlass.Connection;

namespace SmartGlass.Tests
{
    public class TestCrypto : TestDataProvider
    {
        public TestCrypto()
            : base("")
        {
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCryptoSetup()
        {
            byte[] cert = TestData["selfsigned_cert.bin"];
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);

            CryptoContext context = new CryptoContext(x509);

            Assert.IsNotNull(context);
            Assert.AreEqual(PublicKeyType.EC_DH_P256, context.PublicKeyType);
        }
    }
}