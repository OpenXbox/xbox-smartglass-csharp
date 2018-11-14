using NUnit.Framework;
using System.Threading.Tasks;
using SmartGlass.Common;

namespace SmartGlass.Common.Tests
{
    public class AsyncLazyTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAsyncLazy()
        {
            int tmpInt = 99;
            Task<int> ReturnInt42()
            {
                return Task<int>.Run(() => { tmpInt = 42; return tmpInt; });
            }

            AsyncLazy<int> cls = new AsyncLazy<int>(ReturnInt42);

            Assert.AreEqual(99, tmpInt);
            int result42 = cls.GetAsync().GetAwaiter().GetResult();
            Assert.AreEqual(42, tmpInt);
            Assert.AreEqual(42, result42);
        }
    }
}