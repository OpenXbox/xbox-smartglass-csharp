using System;
using System.Threading.Tasks;

namespace SmartGlass.Common
{
    public class AsyncLazy<T>
    {
        private readonly object _lockObject = new object();
        protected object LockObject => _lockObject;

        private readonly Func<Task<T>> _createFunc;

        private Task<T> _value;
        protected Task<T> Value => _value;

        public AsyncLazy(Func<Task<T>> createFunc)
        {
            _createFunc = createFunc;
        }

        public Task<T> GetAsync()
        {
            lock (_lockObject)
            {
                if (_value == null)
                {
                    _value = _createFunc();
                }
            }

            return _value;
        }
    }
}