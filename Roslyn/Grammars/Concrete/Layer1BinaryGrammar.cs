using Roslyn.Expressions;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer1BinaryGrammar : LeftUnionGrammar
    {
        public Layer1BinaryGrammar()
        {
            Secondary = new Layer2BinaryGrammar();
            Build = Base;
        }

        private Expression Base(IEnumerableReader<Cell> reader)
        {
            var pre = Secondary.Build(reader);
            return reader.Peek().Type switch
            {
                CellType.Assign => Assign(pre, reader),
                CellType.Quote => Quote(pre, reader),
                _ => pre
            };
        }

        private Expression Assign(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Assign(pre, Base(reader.Skip(1)));

        private static Expression Quote(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Quote(pre, ExpressionBuilder.Build(reader.Skip(1)));
    }
}
