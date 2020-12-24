using Roslyn.Expressions;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer2BinaryGrammar : RightUnionGrammar
    {
        public Layer2BinaryGrammar()
        {
            Secondary = new Layer3BinaryGrammar();
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
                    CellType.Or => Or(pre, reader),
                    _ => pre
                };
                if (pre == tmp) break;
            }

            return pre;
        }

        private Expression Or(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Or(pre, Secondary.Build(reader.Skip(1)));
    }
}
