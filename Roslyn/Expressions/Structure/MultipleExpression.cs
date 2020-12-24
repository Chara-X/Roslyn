using Roslyn.Expressions.Concrete;
using Roslyn.Tools;

namespace Roslyn.Expressions.Structure
{
    public class MultipleExpression : Expression
    {
        public Expression[] Subs { get; set; }

        internal MultipleExpression(Expression[] subs) => Subs = subs;

        public static MultipleExpression Inline(params Expression[] subs)
        {
            var e = new MultipleExpression(subs);
            e.Value = e.Inline;
            return e;
        }

        private object Inline(ExpressionContext context)
        {
            var inline = new ExpressionContext(new DictionaryRef<string, object>(context.Objects));
            foreach (var sub in Subs)
            {
                if (sub is ReturnExpression)
                    return sub.Value(inline);
                sub.Value(inline);
            }

            return inline;
        }
    }
}
