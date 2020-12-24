using Roslyn.Expressions;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer6BinaryGrammar: RightUnionGrammar
    {
        public Layer6BinaryGrammar() 
        {
            Secondary = new Layer7SuffixGrammar();
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
                    CellType.Multiply => Multiply(pre, reader),
                    _ => pre
                };
                if (pre == tmp) break;
            }

            return pre;
        }

        private Expression Multiply(Expression pre, IEnumerableReader<Cell> reader) =>
            BinaryExpression.Multiply(pre, Secondary.Build(reader.Skip(1)));
    }
}
