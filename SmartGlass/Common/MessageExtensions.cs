using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartGlass.Common
{
    /// <summary>
    /// Message extensions.
    /// Used by classes implementing IMessageTransport.
    /// </summary>
    public static class MessageExtensions
    {
        public static async Task<T> WaitForMessageAsync<T, TError, TBase>(
            this IMessageTransport<TBase> transport, TimeSpan timeout, Action startAction = null, Func<T, bool> filter = null, Func<TError, bool> errorFilter = null)
            where T : TBase
            where TError : TBase, IConvertToException
        {
            return (T)(await TaskExtensions.EventTask<IMessageTransport<TBase>, MessageReceivedEventArgs<TBase>>(
                transport,
                startAction,
                (o, e) => o.MessageReceived += e,
                (o, e) => o.MessageReceived -= e,
                (e) =>
                {
                    if (e.Message is TError)
                    {
                        var error = (TError)e.Message;
                        if (errorFilter == null || !errorFilter(error))
                        {
                            throw error.ToException();
                        }
                    }

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

        /// <summary>
        /// Waits for a specific message in a given timespan.
        /// </summary>
        /// <returns>Awaited message.</returns>
        /// <param name="transport">Transport to listen on.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="startAction">(Optional) Action to trigger before awaiting message.</param>
        /// <param name="filter">(Optional) Filter.</param>
        /// <typeparam name="T">Expected message type.</typeparam>
        /// <typeparam name="TBase">Message baseclass.</typeparam>
        public static async Task<T> WaitForMessageAsync<T, TBase>(
            this IMessageTransport<TBase> transport, TimeSpan timeout, Action startAction = null, Func<T, bool> filter = null)
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

        /// <summary>
        /// Reads messages of specific type in given timespan.
        /// For more-refined filtering, an additional filter function can be
        /// provided.
        /// </summary>
        /// <returns>The read messages.</returns>
        /// <param name="transport">Transport.</param>
        /// <param name="readDuration">Read duration.</param>
        /// <param name="startAction">(Optional) Action to trigger before awaiting message.</param>
        /// <param name="filter">(Optional) Filter.</param>
        /// <typeparam name="T">Expected message type.</typeparam>
        public static IEnumerable<T> ReadMessages<T>(
            this IMessageTransport<T> transport, TimeSpan readDuration, Action startAction = null)
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
                if (startAction != null)
                {
                    startAction();
                }
            }

            // Wait the specified time, then close the buffer to
            // stop receiving any more messages
            Task.Delay(readDuration).ContinueWith(t =>
            {
                transport.MessageReceived -= handler;
                blockingCollection.CompleteAdding();
            });

            return blockingCollection.GetConsumingEnumerable();
        }
    }
}
