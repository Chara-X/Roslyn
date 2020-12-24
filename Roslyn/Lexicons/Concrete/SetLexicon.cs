using System.Collections.Generic;
using Roslyn.Lexicons.Structure;

namespace Roslyn.Lexicons.Concrete
{
    public class SetLexicon : UnaryLexicon
    {
        public HashSet<char> Condition { get; set; }

        internal SetLexicon(Lexicon next, HashSet<char> condition) : base(next) => Condition = condition;

        public static SetLexicon Set(Lexicon next, HashSet<char> condition)
        {
            var lex = new SetLexicon(next, condition);
            lex.Match = lex.Set;
            return lex;
        }

        private Capture<bool> Set(string input, int offset)
        {
            if (offset == input.Length || !Condition.Contains(input[offset])) return new Capture<bool>(false, 0);
            return Next.Match(input, offset + 1);
        }
    }
}
