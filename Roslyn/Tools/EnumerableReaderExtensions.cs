using System;
using System.Collections.Generic;

namespace Roslyn.Tools
{
    public static class EnumerableReaderExtensions
    {
        public static IEnumerable<T> TakeWhile<T>(this IEnumerableReader<T> reader, Func<T, bool> predicate)
        {
            var value = new List<T>();
            while (!reader.End() && predicate(reader.Peek()))
                value.Add(reader.Read());
            return value;
        }

        public static IEnumerableReader<T> SkipWhile<T>(this IEnumerableReader<T> reader, Func<T, bool> predicate)
        {
            while (!reader.End() && predicate(reader.Peek()))
                reader.Read();
            return reader;
        }

        public static IEnumerableReader<T> Skip<T>(this IEnumerableReader<T> reader, int offset)
        {
            for (var i = 0; i < offset; i++)
                reader.Read();
            return reader;
        }
    }
}
