using System;
using System.Threading.Tasks;
using SmartGlass.Common;

namespace SmartGlass.Tests
{
    public class TestFixture : IDisposable
    {
        private bool _disposed = false;

        public TestFixture()
        {
        }

        /* Test classes start */
        public interface ITestMessage
        {

        }

        public class TestMessage1 : ITestMessage
        {
            public int Number;
        }

        public class TestMessage2 : ITestMessage
        {
            public int Decimal;
        }

        public class MessageTransportTestCls : IMessageTransport<ITestMessage>
        {
            public event System.EventHandler<MessageReceivedEventArgs<ITestMessage>> MessageReceived;

            public void DummyConsume(ITestMessage msg)
                => MessageReceived?.Invoke(this, new MessageReceivedEventArgs<ITestMessage>(msg));

            public Task SendAsync(ITestMessage message)
            {
                throw new System.NotImplementedException();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                _disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}