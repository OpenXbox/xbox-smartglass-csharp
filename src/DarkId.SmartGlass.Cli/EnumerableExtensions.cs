using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkId.SmartGlass.Cli
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

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int len)
        {
            if (len == 0)
                throw new ArgumentNullException();

            var enumer = source.GetEnumerator();
            while (enumer.MoveNext())
            {
                yield return Take(enumer.Current, enumer, len);
            }
        }

        private static IEnumerable<T> Take<T>(T head, IEnumerator<T> tail, int len)
        {
            while (true)
            {
                yield return head;
                if (--len == 0)
                    break;
                if (tail.MoveNext())
                    head = tail.Current;
                else
                    break;
            }
        }
    }
}