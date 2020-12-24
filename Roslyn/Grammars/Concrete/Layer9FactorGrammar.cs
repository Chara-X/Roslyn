using System.Collections.Generic;
using Roslyn.Expressions;
using Roslyn.Expressions.Concrete;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;
using Expression = Roslyn.Expressions.Expression;

namespace Roslyn.Grammars.Concrete
{
    public class Layer9FactorGrammar : LayerGrammar
    {
        public Layer9FactorGrammar()  => Build = Base;

        private static Expression Base(IEnumerableReader<Cell> reader)
        {
            return reader.Peek().Type switch
            {
                CellType.L1 => Inline(reader),
                CellType.L3 => Operate(reader),
                CellType.Var => Declare(reader),
                CellType.Digits => Digits(reader),
                CellType.Chars => Chars(reader),
                CellType.Char => Char(reader),
                CellType.True => True(reader),
                CellType.False => False(reader),
                CellType.Typeof => TypeOf(reader),
                CellType.Variable => Variable(reader),
                _ => null
            };
        }

        private static Expression Digits(IEnumerableReader<Cell> reader) => ConstantExpression.Digits(reader.Read().Value);

        private static Expression Chars(IEnumerableReader<Cell> reader) => ConstantExpression.Chars(reader.Read().Value);

        private static Expression Char(IEnumerableReader<Cell> reader) => ConstantExpression.Char(reader.Read().Value);

        private static Expression True(IEnumerableReader<Cell> reader) => ConstantExpression.True(reader.Read().Value);

        private static Expression False(IEnumerableReader<Cell> reader) => ConstantExpression.False(reader.Read().Value);

        private static Expression TypeOf(IEnumerableReader<Cell> reader) => ConstantExpression.Typeof(reader.Read().Value);

        private static Expression Declare(IEnumerableReader<Cell> reader) => VariableExpression.Declare(reader.Skip(1).Read().Value);

        private static Expression Variable(IEnumerableReader<Cell> reader) => VariableExpression.Load(reader.Read().Value);

        private static Expression Inline(IEnumerableReader<Cell> reader)
        {
            var es = new List<Expression>();
            while (reader.Read().Type != CellType.R1)
                if (reader.Peek().Type != CellType.R1)
                    es.Add(ExpressionBuilder.Build(reader));
            return MultipleExpression.Inline(es.ToArray());
        }

        private static Expression Operate(IEnumerableReader<Cell> reader)
        {
            var e = ExpressionBuilder.Build(reader.Skip(1));
            reader.Skip(1);
            return e;
        }
    }
}
