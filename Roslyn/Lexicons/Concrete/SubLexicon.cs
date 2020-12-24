using Roslyn.Lexicons.Structure;

namespace Roslyn.Lexicons.Concrete
{
    public class SubLexicon : UnaryLexicon
    {
        public Lexicon[] Condition { get; set; }

        internal SubLexicon(Lexicon next, Lexicon[] conditions) : base(next) => Condition = conditions;

        public static SubLexicon Sub(Lexicon next, Lexicon[] condition)
        {
            var lex = new SubLexicon(next, condition);
            lex.Match = lex.Sub;
            return lex;
        }

        private Capture<bool> Sub(string input, int offset)
        {
            foreach (var sub in Condition)
            {
                var capture = sub.Match(input, offset);
                if (!capture.Result) continue;
                capture = Next.Match(input, capture.Length);
                if (!capture.Result) continue;
                return capture;
            }

            return new Capture<bool>(false, 0);
        }
    }
}
