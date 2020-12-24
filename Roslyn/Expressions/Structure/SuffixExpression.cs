namespace Roslyn.Expressions.Structure
{
    public class SuffixExpression : UnaryExpression
    {
        internal SuffixExpression(Expression sub) : base(sub)
        {
        }
    }
}
