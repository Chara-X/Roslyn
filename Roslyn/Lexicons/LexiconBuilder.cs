using System.Collections.Generic;
using System.Linq;
using Roslyn.Lexicons.Concrete;
using Roslyn.Lexicons.Structure;
using Roslyn.Tools;

namespace Roslyn.Lexicons
{
    public class LexiconBuilder
    {
        public static Lexicon Build(string pattern) => Base(new EnumerableReader<char>(pattern.ToCharArray()));

        private static Lexicon Base(IEnumerableReader<char> reader)
        {
            if (reader.End()) return End();
            return reader.Peek() switch
            {
                '[' => Set(reader),
                '(' => Sub(reader),
                '.' => Any(reader),
                '*' => More(reader),
                '#' => Less(reader),
                ']' => End(),
                ')' => End(),
                '}' => End(),
                '|' => End(),
                '\\' => Some(reader.Skip(1)),
                _ => Some(reader)
            };
        }

        private static Lexicon End() => Lexicon.End();

        private static Lexicon Some(IEnumerableReader<char> reader) => OneLexicon.Some(reader.Read(), Base(reader));

        private static Lexicon Any(IEnumerableReader<char> reader) => OneLexicon.Any(reader.Read(), Base(reader));

        private static Lexicon Set(IEnumerableReader<char> reader)
        {
            var content = new string(reader.Skip(1).TakeWhile(i => i != ']').ToArray());
            var set = new HashSet<char>();
            foreach (var s in content.Split('|'))
                for (var i = s[0]; i <= s[2]; i++)
                    if (!set.Contains(i))
                        set.Add(i);
            return SetLexicon.Set(Base(reader.Skip(1)), set);
        }

        private static Lexicon Sub(IEnumerableReader<char> reader)
        {
            var subs = new List<Lexicon>();
            while (reader.Read() != ')')
                subs.Add(Base(reader));
            return SubLexicon.Sub(Base(reader), subs.ToArray());
        }

        private static Lexicon More(IEnumerableReader<char> reader) => BinaryLexicon.More((UnaryLexicon)Base(reader.Skip(2)), Base(reader.Skip(1)));

        private static Lexicon Less(IEnumerableReader<char> reader) => BinaryLexicon.Less((UnaryLexicon)Base(reader.Skip(2)), Base(reader.Skip(1)));
    }
}
