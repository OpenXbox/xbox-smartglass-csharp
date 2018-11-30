using System.Collections.Generic;
using SmartGlass;
using SmartGlass.Common;
using SmartGlass.Connection;
using Tests.Resources;
using Xunit;

namespace Tests.SmartGlass
{
    public class TestCrypto
    {
        public TestCrypto()
        {
        }

        [Fact]
        public void TestCryptoSetup()
        {
            byte[] cert = ResourcesProvider.GetContent("selfsigned_cert.bin");
            var x509 = CryptoExtensions.DeserializeCertificateAsn(cert);

            CryptoContext context = new CryptoContext(x509);

            Assert.NotNull(context);
            Assert.Equal<PublicKeyType>(PublicKeyType.EC_DH_P256, context.PublicKeyType);
        }
    }
}