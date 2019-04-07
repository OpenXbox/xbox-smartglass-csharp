using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartGlass;
using SmartGlass.Common;
using SmartGlass.Connection;
using SmartGlass.Messaging;
using SmartGlass.Tests.Resources;
using Xunit;
using static SmartGlass.Tests.TestFixture;
using TaskExtensions = SmartGlass.Common.TaskExtensions;

namespace SmartGlass.Tests
{
    public class TestTaskExtensions
    {
        readonly TimeSpan[] fastRetryPolicy = new TimeSpan[]{
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(150),
            TimeSpan.FromMilliseconds(200),
            TimeSpan.FromMilliseconds(250)
        };

        [Fact]
        public async void TestWithRetries()
        {
            var dtNow = DateTime.Now;
            await TaskExtensions.WithRetries(
                () => Task.Run(() => {}),
                fastRetryPolicy,
                (t) => true
            );
        }
    }
}