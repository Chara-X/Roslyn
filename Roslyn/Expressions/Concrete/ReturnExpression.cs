using Roslyn.Expressions.Structure;

namespace Roslyn.Expressions.Concrete
{
    public class ReturnExpression : UnaryExpression
    {
        internal ReturnExpression(Expression sub) : base(sub){}

        public static ReturnExpression Return(Expression sub)
        {
            var e = new ReturnExpression(sub);
            e.Value = e.Return;
            return e;
        }

        private object Return(ExpressionContext context) => Sub.Value(context);
    }
}
