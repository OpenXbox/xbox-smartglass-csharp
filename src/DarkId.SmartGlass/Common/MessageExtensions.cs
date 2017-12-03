using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DarkId.SmartGlass.Common
{
    internal static class MessageExtensions
    {
        public static async Task<T> WaitForMessageAsync<T, TBase>(
            this IMessageTransport<TBase> transport, TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : TBase
        {
            return (T)(await TaskExtensions.EventTask<IMessageTransport<TBase>, MessageReceivedEventArgs<TBase>>(
                transport,
                startAction,
                (o, e) => o.MessageReceived += e,
                (o, e) => o.MessageReceived -= e,
                (e) =>
                {
                    if (e.Message is T)
                    {
                        if (filter != null)
                        {
                            return filter((T)e.Message);
                        }

                        return true;
                    }

                    return false;
                },
                timeout)).Message;
        }

        public static IEnumerable<T> ReadMessages<T>(
            this IMessageTransport<T> transport, TimeSpan readDuration, Action startAction)
        {
            var lockObject = new object();
            var blockingCollection = new BlockingCollection<T>();

            EventHandler<MessageReceivedEventArgs<T>> handler = (s, e) =>
            {
                lock (lockObject)
                {
                    blockingCollection.Add(e.Message);
                }
            };

            lock (lockObject)
            {
                transport.MessageReceived += handler;
                startAction();
            }

            Task.Delay(readDuration).ContinueWith(t =>
            {
                transport.MessageReceived -= handler;
                blockingCollection.CompleteAdding();
            });

            return blockingCollection.GetConsumingEnumerable();
        }
    }
}