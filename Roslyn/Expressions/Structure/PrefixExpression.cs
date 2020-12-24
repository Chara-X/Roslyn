namespace Roslyn.Expressions.Structure
{
    public class PrefixExpression : UnaryExpression
    {
        internal PrefixExpression(Expression sub) : base(sub)
        {
        }
    }
}
