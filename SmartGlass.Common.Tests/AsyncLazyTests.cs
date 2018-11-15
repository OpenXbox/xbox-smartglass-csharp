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

            AsyncLazy<int> cls = new AsyncLazy<int>(() =>
            {
                return Task<int>.Run(() => { tmpInt = 42; return tmpInt; });
            });

            Assert.AreEqual(99, tmpInt);
            int result42 = cls.GetAsync().GetAwaiter().GetResult();

            Assert.AreEqual(42, tmpInt);
            Assert.AreEqual(42, result42);
        }

        [Test]
        public void TestAsyncLazyException()
        {
            AsyncLazy<int> cls = new AsyncLazy<int>(() =>
            {
                throw new System.DivideByZeroException("");
            });

            Assert.ThrowsAsync<System.DivideByZeroException>(cls.GetAsync);
        }
    }
}