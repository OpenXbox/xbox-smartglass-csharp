using System.Collections.Generic;
using SmartGlass.Common;
using SmartGlass.Connection;
using Xunit;

namespace SmartGlass.Tests
{
    public class TestCrypto : TestDataProvider
    {
        public TestCrypto()
            : base("")
        {
        }

        [Fact]
        public void TestCryptoSetup()
        {
            byte[] cert = TestData["selfsigned_cert.bin"];
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);

            CryptoContext context = new CryptoContext(x509);

            Assert.NotNull(context);
            Assert.Equal<PublicKeyType>(PublicKeyType.EC_DH_P256, context.PublicKeyType);
        }
    }
}