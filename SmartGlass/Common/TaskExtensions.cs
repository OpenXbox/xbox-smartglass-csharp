using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass
{
    internal static class TaskExtensions
    {
        public static Task<TEventArgs> EventTask<T, TEventArgs>(
            T obj,
            Action postAddAction,
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
                postAddAction();
            }

            Task.Delay(timeout, timeoutCancellation.Token).ContinueWith(t =>
            {
                if (timeoutCancellation.IsCancellationRequested)
                {
                    return;
                }

                tcs.TrySetException(new TimeoutException());
                remove(obj, handler);
            });

            return tcs.Task;
        }

        public static Task WithRetries(Func<Task> func,
            IEnumerable<TimeSpan> retryIntervals,
            Func<Task, bool> canRetry = null)
        {
            return WithRetries<object>(async () => { await func(); return null; }, retryIntervals, canRetry);
        }

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

        public static async Task When<T>(this Task<T> task)
        {
            await Task.WhenAny(EnumerableExtensions.EnumerableOf(task));
        }
    }
}