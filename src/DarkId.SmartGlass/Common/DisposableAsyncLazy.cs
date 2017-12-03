using System;
using System.Threading.Tasks;

namespace DarkId.SmartGlass.Common
{
    internal class DisposableAsyncLazy<T> : AsyncLazy<T>, IDisposable
        where T : IDisposable
    {
        private bool _isDisposed;

        public DisposableAsyncLazy(Func<Task<T>> createFunc)
            : base(createFunc)
        {
        }

        public void Dispose()
        {
            lock (LockObject)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            if (Value != null)
            {
                Value.When().Wait();

                if (Value.Status == TaskStatus.RanToCompletion)
                {
                    Value.Result.Dispose();
                }
            }
        }
    }
}