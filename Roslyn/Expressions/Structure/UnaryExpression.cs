namespace Roslyn.Expressions.Structure
{
    public class UnaryExpression : Expression
    {
        public Expression Sub { get; set; }

        internal UnaryExpression(Expression sub) => Sub = sub;
    }
}
