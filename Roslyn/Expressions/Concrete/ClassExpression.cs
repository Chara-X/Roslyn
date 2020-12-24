using Roslyn.Expressions.Structure;

namespace Roslyn.Expressions.Concrete
{
    public class ClassExpression : PrefixExpression
    {
        public string Name { get; set; }

        internal ClassExpression(string name, Expression sub) : base(sub) => Name = name;

        public static ClassExpression New(string name, Expression sub)
        {
            var e = new ClassExpression(name, sub);
            e.Value = e.New;
            return e;
        }

        private object New(ExpressionContext context) => Sub.Value(new ExpressionContext());
    }
}
