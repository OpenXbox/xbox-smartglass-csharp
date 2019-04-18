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

namespace SmartGlass.Tests
{
    public class TestMessageExtensions
    {
        readonly MessageTransportTestCls _transport;
        readonly List<ITestMessage> _store;

        readonly TestMessage1 TestMessage1Object = new TestMessage1() { Number = 42 };
        readonly TestMessage2 TestMessage2Object = new TestMessage2() { Decimal = 99 };

        public TestMessageExtensions()
        {
            _store = new List<ITestMessage>();
            _transport = new MessageTransportTestCls();

            _transport.MessageReceived += (sender, arg) => _store.Add(arg.Message);
        }


        [Fact]
        public async void TestWaitForMessageInstant()
        {
            var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                TimeSpan.FromSeconds(2),
                () => Task.Run(() => _transport.DummyConsume(TestMessage1Object)));

            Assert.True(_store.Count == 1);
            Assert.Equal(TestMessage1Object, result);
            Assert.Equal(TestMessage1Object, _store[0]);
        }

        [Fact]
        public async void TestWaitForMessageSendOutsideAction()
        {
            var resultTask = _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                TimeSpan.FromSeconds(2),
                null);

            _transport.DummyConsume(TestMessage1Object);
            var result = await resultTask;

            Assert.True(_store.Count == 1);
            Assert.Equal(TestMessage1Object, result);
            Assert.Equal(TestMessage1Object, _store[0]);
        }

        public async void TestWaitForMessageLongerAsyncActionThanTimeout()
        {
            var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                TimeSpan.FromSeconds(1),
                () => Task.Run(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    _transport.DummyConsume(TestMessage1Object);
                })
            );

            Assert.True(_store.Count == 1);
            Assert.Equal(TestMessage1Object, _store[0]);
        }

        [Fact]
        public async void TestWaitForMessageLongerActionThanTimeout()
        {
            var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                TimeSpan.FromSeconds(1),
                () => Task.Run(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
                    _transport.DummyConsume(TestMessage1Object);
                })
            );

            Assert.True(_store.Count == 1);
            Assert.Equal(TestMessage1Object, _store[0]);
        }

        [Fact]
        public async void TestWaitForMessageFilterMatching()
        {
            var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                TimeSpan.FromMilliseconds(100),
                () => Task.Run(() => _transport.DummyConsume(TestMessage1Object)),
                (msg) => msg.Number == 42);

            Assert.True(_store.Count == 1);
            Assert.Equal(result, TestMessage1Object);
        }

        [Fact]
        public async void TestWaitForMessageFilterNotMatching()
        {
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                    TimeSpan.FromMilliseconds(100),
                    () => Task.Run(() => _transport.DummyConsume(TestMessage1Object)),
                    (msg) => msg.Number == 101);
            });

            Assert.True(_store.Count == 1);
        }

        [Fact]
        public async void TestWaitForMessageWrongMessage()
        {
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                    TimeSpan.FromMilliseconds(100),
                    () => Task.Run(() => _transport.DummyConsume(TestMessage2Object)));
            });

            Assert.True(_store.Count == 1);
        }

        [Fact]
        public async void TestWaitForMessageActionThrowsException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                    TimeSpan.FromMilliseconds(100),
                    () => Task.Run(() => throw new InvalidOperationException()));
            });

            Assert.True(_store.Count == 0);
        }

        [Fact]
        public async void TestWaitForMessageNoMessage()
        {
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                    TimeSpan.FromMilliseconds(100));
            });

            Assert.True(_store.Count == 0);
        }

        [Fact]
        public async void TestWaitForMessageMultipleBeforeMatching()
        {
            var result = await _transport.WaitForMessageAsync<TestMessage1, ITestMessage>(
                TimeSpan.FromMilliseconds(100),
                () => Task.Run(() =>
                {
                    _transport.DummyConsume(TestMessage2Object);
                    _transport.DummyConsume(TestMessage2Object);
                    _transport.DummyConsume(TestMessage2Object);
                    _transport.DummyConsume(TestMessage2Object);
                    _transport.DummyConsume(TestMessage2Object);
                    _transport.DummyConsume(TestMessage1Object);
                }));

            Assert.True(_store.Count == 6);
            Assert.Equal(TestMessage1Object, result);
            Assert.Equal(TestMessage1Object, _store[5]);
        }
    }
}