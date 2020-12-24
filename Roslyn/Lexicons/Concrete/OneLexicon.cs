using Roslyn.Lexicons.Structure;

namespace Roslyn.Lexicons.Concrete
{
    public class OneLexicon : UnaryLexicon
    {
        public char Condition { get; set; }

        internal OneLexicon(Lexicon next, char condition) : base(next) => Condition = condition;

        public static OneLexicon Some(char condition, Lexicon next)
        {
            var lex = new OneLexicon(next, condition);
            lex.Match = lex.Some;
            return lex;
        }

        public static OneLexicon Any(char condition, Lexicon next)
        {
            var lex = new OneLexicon(next, condition);
            lex.Match = lex.Any;
            return lex;
        }

        private Capture<bool> Some(string input, int offset)
        {
            if (offset == input.Length || input[offset] != Condition) return new Capture<bool>(false, 0);
            return Next.Match(input, offset + 1);
        }

        private Capture<bool> Any(string input, int offset)
        {
            return offset == input.Length ? new Capture<bool>(false, 0) : Next.Match(input, offset + 1);
        }
    }
}
