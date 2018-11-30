using System.Threading.Tasks;
using SmartGlass.Common;
using Xunit;

namespace SmartGlass.Common.Tests
{
    public class AsyncLazyTests
    {
        [Fact]
        public void TestAsyncLazy()
        {
            int tmpInt = 99;

            AsyncLazy<int> cls = new AsyncLazy<int>(() =>
            {
                return Task<int>.Run(() => { tmpInt = 42; return tmpInt; });
            });

            Assert.Equal(99, tmpInt);
            int result42 = cls.GetAsync().GetAwaiter().GetResult();

            Assert.Equal(42, tmpInt);
            Assert.Equal(42, result42);
        }

        [Fact]
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