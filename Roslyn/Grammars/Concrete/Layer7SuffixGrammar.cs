using System;
using System.Collections.Generic;
using Roslyn.Expressions;
using Roslyn.Expressions.Concrete;
using Roslyn.Grammars.Structure;
using Roslyn.Tools;

namespace Roslyn.Grammars.Concrete
{
    public class Layer7SuffixGrammar : RightUnionGrammar
    {
        public Layer7SuffixGrammar() 
        {
            Secondary = new Layer8PrefixGrammar();
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
                    CellType.Period => MemberRef(pre, reader),
                    CellType.Arrow => Member(pre, reader),
                    CellType.L3 => Execute(pre, reader),
                    _ => pre
                };
                if (pre == tmp) break;
            }

            return pre;
        }

        private static Expression MemberRef(Expression pre, IEnumerableReader<Cell> reader) => MemberExpression.LoadRef(pre, reader.Skip(1).Read().Value);

        private static Expression Member(Expression pre, IEnumerableReader<Cell> reader) => MemberExpression.Load(pre, reader.Skip(1).Read().Value);

        private static Expression Execute(Expression pre, IEnumerableReader<Cell> reader)
        {
            var es = new List<Expression>();
            while (reader.Read().Type != CellType.R3)
                if (reader.Peek().Type != CellType.R3)
                    es.Add(ExpressionBuilder.Build(reader));
            return ExecuteExpression.Exec(pre, es.ToArray());
        }
    }
}
