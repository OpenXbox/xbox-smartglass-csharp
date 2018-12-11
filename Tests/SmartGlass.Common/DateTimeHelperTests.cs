using System;
using SmartGlass.Common;
using Xunit;

namespace Tests.SmartGlass.Common
{
    public class DateTimeHelperTests
    {
        DateTime _expected;

        public DateTimeHelperTests()
        {
            _expected = new DateTime(2018, 12, 11, 12, 44, 47, DateTimeKind.Utc)
                .AddMilliseconds(437);
        }

        [Fact]
        public void TestFromEpochMilliseconds()
        {
            // December 11, 2018, 12:44:47.4370000 UTC
            DateTime dt = DateTimeHelper.FromEpochMilliseconds(1544532287437);
            Assert.Equal(_expected, dt.ToUniversalTime());
        }

        [Fact]
        public void TestToEpochMilliseconds()
        {
            ulong millisecondsEpoch = DateTimeHelper.ToEpochMilliseconds(_expected);
            DateTime result = DateTimeHelper.FromEpochMilliseconds(millisecondsEpoch);

            Assert.Equal(_expected, result);
        }

        [Fact]
        public void TestToTimestampMilliseconds()
        {
            DateTime future1234ms = _expected + TimeSpan.FromMilliseconds(1234);
            ulong ts = DateTimeHelper.ToTimestampMilliseconds(future1234ms, _expected);

            Assert.Equal((ulong)1234, ts);
        }

        [Fact]
        public void TestToTimestampMicroseconds()
        {
            DateTime future1234ms = _expected + TimeSpan.FromMilliseconds(1234);
            ulong ts = DateTimeHelper.ToTimestampMicroseconds(future1234ms, _expected);

            Assert.Equal((ulong)1234000, ts);
        }

        [Fact]
        public void TestFromTimestampMilliseconds()
        {
            DateTime refTimestamp = new DateTime(2018, 12, 11, 8, 30, 22, DateTimeKind.Utc);
            DateTime expect = refTimestamp.AddMilliseconds(500);

            DateTime result = DateTimeHelper.FromTimestampMilliseconds(500, refTimestamp);

            Assert.Equal(expect, result);
        }

        [Fact]
        public void TestFromTimestampMicroseconds()
        {
            DateTime refTimestamp = new DateTime(2018, 12, 11, 8, 30, 22, DateTimeKind.Utc);
            DateTime expect = refTimestamp.AddMilliseconds(500);

            DateTime result = DateTimeHelper.FromTimestampMicroseconds(500 * 1000, refTimestamp);

            Assert.Equal(expect, result);
        }
    }
}