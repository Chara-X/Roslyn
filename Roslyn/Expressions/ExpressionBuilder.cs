using Roslyn.Grammars;
using Roslyn.Grammars.Concrete;
using Roslyn.Tools;

namespace Roslyn.Expressions
{
    public static class ExpressionBuilder
    {
        private static readonly Grammar Grammar = new Layer0StatementGrammar();

        public static Expression Build(string code) => Grammar[code];

        public static Expression Build(IEnumerableReader<Cell> reader) => Grammar.Build(reader);
    }
}
