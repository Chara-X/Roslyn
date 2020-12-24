using System.Collections.Generic;
using Roslyn.Expressions;
using Roslyn.Expressions.Concrete;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer8PrefixGrammar : LayerGrammar
    {
        public Layer8PrefixGrammar() 
        {
            Secondary = new Layer9FactorGrammar();
            Build = Base;
        }

        private Expression Base(IEnumerableReader<Cell> reader)
        {
            return reader.Peek().Type switch
            {
                CellType.Class => Class(reader),
                CellType.Function => Function(reader),
                _ => Secondary.Build(reader)
            };
        }

        private Expression Class(IEnumerableReader<Cell> reader) => ClassExpression.New(reader.Skip(1).Read().Value, Secondary.Build(reader));

        private Expression Function(IEnumerableReader<Cell> reader)
        {
            var variables = new List<string>();
            reader.Skip(1);
            while (reader.Read().Type != CellType.R3)
                if (reader.Peek().Type != CellType.R3)
                    variables.Add(reader.Read().Value);
            return FunctionExpression.Run(variables.ToArray(), Secondary.Build(reader));
        }
    }
}
