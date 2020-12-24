namespace Roslyn.Expressions.Structure
{
    public class TernaryExpression : Expression
    {
        public Expression Test { get; set; }
        public Expression True { get; set; }
        public Expression False { get; set; }

        internal TernaryExpression(Expression test, Expression @true, Expression @false)
        {
            Test = test;
            True = @true;
            False = @false;
        }

        public static TernaryExpression Condition(Expression test, Expression @true, Expression @false)
        {
            var e = new TernaryExpression(test, @true, @false);
            e.Value = e.Condition;
            return e;
        }

        public static TernaryExpression Loop(Expression test, Expression @true)
        {
            var e = new TernaryExpression(test, @true, null);
            e.Value = e.Loop;
            return e;
        }

        private object Condition(ExpressionContext context)
        {
            if ((bool) Test.Value(context))
                True.Value(context);
            else
                False.Value(context);
            return null;
        }

        private object Loop(ExpressionContext context)
        {
            while ((bool) Test.Value(context))
                True.Value(context);
            return null;
        }
    }
}
