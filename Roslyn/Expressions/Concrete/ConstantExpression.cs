using Roslyn.Shared;

namespace Roslyn.Expressions.Concrete
{
    public class ConstantExpression : Expression
    {
        public string Content { get; set; }

        public ConstantExpression(string content) => Content = content;

        public static ConstantExpression Digits(string content)
        {
            var e = new ConstantExpression(content);
            e.Value = e.Digits;
            return e;
        }

        public static ConstantExpression Chars(string content)
        {
            var e = new ConstantExpression(content);
            e.Value = e.Chars;
            return e;
        }

        public static ConstantExpression Char(string content)
        {
            var e = new ConstantExpression(content);
            e.Value = e.Char;
            return e;
        }

        public static ConstantExpression Typeof(string content)
        {
            var e = new ConstantExpression(content);
            e.Value = e.Typeof;
            return e;
        }

        public static ConstantExpression True(string content) => new ConstantExpression(content) {Value = True};

        public static ConstantExpression False(string content) => new ConstantExpression(content) {Value = False};

        private object Digits(ExpressionContext context) => int.Parse(Content);

        private object Chars(ExpressionContext context) => Content;

        private object Char(ExpressionContext context) => Content[0];

        private object Typeof(ExpressionContext context) => Assembly.GetType(Content);

        private static object True(ExpressionContext context) => true;

        private static object False(ExpressionContext context) => false;
    }
}
