using Roslyn.Expressions;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer4BinaryGrammar : RightUnionGrammar
    {
        public Layer4BinaryGrammar()
        {
            Secondary = new Layer5BinaryGrammar();
            Build = Base;
        }

        private Expression Base(IEnumerableReader<Cell> reader)
        {
            var pre = Secondary.Build(reader);
            while (true)
            {
                var tmp = pre;
                pre = reader.Peek().Type switch
                {
                    CellType.Equal => Equal(pre, reader),
                    CellType.NotEqual => NotEqual(pre, reader),
                    CellType.Greater => Greater(pre, reader),
                    CellType.Less => Less(pre, reader),
                    CellType.GreaterEqual => GreaterEqual(pre, reader),
                    CellType.LessEqual => LessEqual(pre, reader),
                    _ => pre
                };
                if (pre == tmp) break;
            }

            return pre;
        }

        private Expression Equal(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Equal(pre, Secondary.Build(reader.Skip(1)));

        private Expression NotEqual(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.NotEqual(pre, Secondary.Build(reader.Skip(1)));

        private Expression Greater(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Greater(pre, Secondary.Build(reader.Skip(1)));

        private Expression Less(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Less(pre, Secondary.Build(reader.Skip(1)));

        private Expression GreaterEqual(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.GreaterEqual(pre, Secondary.Build(reader.Skip(1)));

        private Expression LessEqual(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.LessEqual(pre, Secondary.Build(reader.Skip(1)));
    }
}
