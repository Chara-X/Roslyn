using Roslyn.Expressions.Structure;

namespace Roslyn.Expressions.Concrete
{
    public class FunctionExpression : PrefixExpression
    {
        public string[] Names { get; set; }

        internal FunctionExpression(string[] names, Expression sub) : base(sub) => Names = names;

        public static FunctionExpression Run(string[] names, Expression sub)
        {
            var e = new FunctionExpression(names, sub);
            e.Value = e.Sub.Value;
            return e;
        }

        public object Invoke(object[] parameters)
        {
            var context = new ExpressionContext();
            for (var i = 0; i < Names.Length; i++)
                context.Add(Names[i], parameters[i]);
            return Value(context);
        }
    }
}
