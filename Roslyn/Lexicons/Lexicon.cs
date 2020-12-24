using System;

namespace Roslyn.Lexicons
{
    public class Lexicon
    {
        public Func<string, int, Capture<bool>> Match { get; set; }

        internal Lexicon() { }

        public static Lexicon End() => new Lexicon {Match = End};

        private static Capture<bool> End(string input, int offset) => new Capture<bool>(true, offset);

        public Capture<bool> this[string input] => Match(input, 0);
    }

    public class Capture<T>
    {
        public T Result { get; set; }
        public int Length { get; set; }

        public Capture(T result, int length)
        {
            Result = result;
            Length = length;
        }
    }
}
