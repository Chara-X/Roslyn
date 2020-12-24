using Roslyn.Expressions;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer3BinaryGrammar : RightUnionGrammar
    {
        public Layer3BinaryGrammar()
        {
            Secondary = new Layer4BinaryGrammar();
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
                    CellType.And => And(pre, reader),
                    _ => pre
                };
                if (pre == tmp) break;
            }

            return pre;
        }

        private Expression And(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.And(pre, Secondary.Build(reader.Skip(1)));
    }
}
