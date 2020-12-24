namespace Roslyn.Lexicons.Structure
{
    public class BinaryLexicon : Lexicon
    {
        public Lexicon Left { get; set; }
        public Lexicon Right { get; set; }

        internal BinaryLexicon(Lexicon left, Lexicon right)
        {
            Left = left;
            Right = right;
        }

        public static BinaryLexicon More(UnaryLexicon left, Lexicon right)
        {
            var lex = new BinaryLexicon(left, right);
            lex.Match = lex.More;
            left.Next = lex;
            return lex;
        }

        public static BinaryLexicon Less(UnaryLexicon left, Lexicon right)
        {
            var lex = new BinaryLexicon(left, right);
            lex.Match = lex.Less;
            left.Next = lex;
            return lex;
        }

        private Capture<bool> More(string input, int offset)
        {
            var capture = Left.Match(input, offset);
            return capture.Result ? capture : Right.Match(input, offset);
        }

        private Capture<bool> Less(string input, int offset)
        {
            var capture = Right.Match(input, offset);
            return capture.Result ? capture : Left.Match(input, offset);
        }
    }
}
