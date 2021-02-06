using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGlass.Common
{
    /// <summary>
    /// Task extensions.
    /// </summary>
    public static class TaskExtensions
    {
        public static async Task<TEventArgs> EventTask<T, TEventArgs>(
            T obj,
            Func<Task> postAddAction,
            Action<T, EventHandler<TEventArgs>> add,
            Action<T, EventHandler<TEventArgs>> remove,
            Func<TEventArgs, bool> filter,
            TimeSpan timeout)
            where TEventArgs : EventArgs
        {
            var tcs = new TaskCompletionSource<TEventArgs>();
            var timeoutCancellation = new CancellationTokenSource();

            EventHandler<TEventArgs> handler = null;
            handler = (s, e) =>
            {
                if (!filter(e))
                {
                    return;
                }

                timeoutCancellation.Cancel();
                tcs.TrySetResult(e);
                remove(obj, handler);
            };

            add(obj, handler);

            if (postAddAction != null)
            {
                await Task.Run(async () => await postAddAction());
            }

#pragma warning disable CS4014
            Task.Delay(timeout, timeoutCancellation.Token).ContinueWith(t =>
            {
                if (timeoutCancellation.IsCancellationRequested)
                {
                    return;
                }

                tcs.TrySetException(new TimeoutException());
                remove(obj, handler);
            });

            return await tcs.Task;
        }

        /// <summary>
        /// Execute a specific task with provided retry policy.
        /// </summary>
        /// <returns>A task.</returns>
        /// <param name="func">Function to execute.</param>
        /// <param name="retryIntervals">Retry intervals.</param>
        /// <param name="canRetry">Can retry.</param>
        public static Task WithRetries(Func<Task> func,
                IEnumerable<TimeSpan> retryIntervals,
                Func<Task, bool> canRetry = null)
        {
            return WithRetries<object>(async () => { await func(); return null; }, retryIntervals, canRetry);
        }

        /// <summary>
        /// Execute a specific task with provided retry policy.
        /// </summary>
        /// <returns>A task returning a desired object.</returns>
        /// <param name="func">Function to execute.</param>
        /// <param name="retryIntervals">Retry intervals.</param>
        /// <param name="canRetry">Can retry.</param>
        public static async Task<T> WithRetries<T>(
            Func<Task<T>> func,
            IEnumerable<TimeSpan> retryIntervals,
            Func<Task<T>, bool> canRetry = null)
        {
            Task<T> task = func();

            foreach (var interval in retryIntervals)
            {
                await task.When();

                if (task.IsCanceled)
                {
                    break;
                }

                if (task.IsFaulted && (canRetry == null || canRetry(task)))
                {
                    await Task.Delay(interval);
                    task = func();
                }
                else
                {
                    break;
                }
            }

            return await task;
        }

        /// <summary>
        /// Check if a task is an Enumerable of provided type.
        /// </summary>
        /// <returns>A task.</returns>
        /// <param name="task">Parameterized Task.</param>
        /// <typeparam name="T">Expected type.</typeparam>
        public static async Task When<T>(this Task<T> task)
        {
            await Task.WhenAny(EnumerableExtensions.EnumerableOf(task));
        }
    }
}
