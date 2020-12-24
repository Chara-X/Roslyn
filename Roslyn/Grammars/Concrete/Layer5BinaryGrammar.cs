using Roslyn.Expressions;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer5BinaryGrammar : RightUnionGrammar
    {
        public Layer5BinaryGrammar()
        {
            Secondary = new Layer6BinaryGrammar();
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
                    CellType.Add => Add(pre, reader),
                    CellType.Subtract => Subtract(pre, reader),
                    _ => pre
                };
                if (pre == tmp) break;
            }

            return pre;
        }

        private Expression Add(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Add(pre, Secondary.Build(reader.Skip(1)));

        private Expression Subtract(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Subtract(pre, Secondary.Build(reader.Skip(1)));
    }
}
