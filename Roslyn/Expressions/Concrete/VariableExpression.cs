namespace Roslyn.Expressions.Concrete
{
    public class VariableExpression:Expression
    {
        public string Name { get; set; }

        internal VariableExpression(string name) => Name = name;

        public static VariableExpression Declare(string name)
        {
            var e = new VariableExpression(name);
            e.Value = (i) => e.Declare(i);
            e.Refer = e.Declare;
            return e;
        }

        public static VariableExpression Load(string name)
        {
            var e = new VariableExpression(name);
            e.Value = (i) => e.Load(i);
            e.Refer = e.Load;
            return e;
        }

        private ref object Declare(ExpressionContext context)
        {
            context.Add(Name, null);
            return ref context.Last();
        }

        private ref object Load(ExpressionContext context) => ref context[Name];
    }
}
