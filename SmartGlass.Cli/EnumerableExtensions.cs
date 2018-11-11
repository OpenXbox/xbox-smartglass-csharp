using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartGlass.Cli
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TResult> TrySelect<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(v =>
            {
                try
                {
                    return selector(v);
                }
                catch
                {
                    return default(TResult);
                }
            });
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int length)
        {
            if (length == 0)
            {
                throw new ArgumentNullException();
            }

            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return Take(enumerator.Current, enumerator, length);
            }
        }

        private static IEnumerable<T> Take<T>(T head, IEnumerator<T> tail, int length)
        {
            while (true)
            {
                yield return head;

                if (--length == 0)
                {
                    break;
                }

                if (tail.MoveNext())
                {
                    head = tail.Current;
                }
                else
                {
                    break;
                }
            }
        }
    }
}