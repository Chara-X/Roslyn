using Roslyn.Expressions;
using Roslyn.Expressions.Concrete;
using Roslyn.Expressions.Structure;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer0StatementGrammar : LayerGrammar
    {
        public Layer0StatementGrammar()
        {
            Secondary = new Layer1BinaryGrammar();
            Build = Base;
        }

        private Expression Base(IEnumerableReader<Cell> reader)
        {
            return reader.Peek().Type switch
            {
                CellType.If => Condition(reader),
                CellType.While => Loop(reader),
                CellType.Return => Return(reader),
                _ => Secondary.Build(reader)
            };
        }

        private Expression Condition(IEnumerableReader<Cell> reader) =>
            TernaryExpression.Condition(Base(reader.Skip(1)), Base(reader), Base(reader.Skip(1)));

        private Expression Loop(IEnumerableReader<Cell> reader) =>
            TernaryExpression.Loop(Base(reader.Skip(1)), Base(reader));

        private Expression Return(IEnumerableReader<Cell> reader) =>
            ReturnExpression.Return(Secondary.Build(reader.Skip(1)));
    }
}
