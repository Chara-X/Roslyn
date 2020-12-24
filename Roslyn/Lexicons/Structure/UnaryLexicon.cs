namespace Roslyn.Lexicons.Structure
{
    public class UnaryLexicon : Lexicon
    {
        public Lexicon Next { get; set; }

        internal UnaryLexicon(Lexicon next) => Next = next;
    }
}
