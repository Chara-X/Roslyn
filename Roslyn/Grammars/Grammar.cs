using System;
using Roslyn.Expressions;
using Roslyn.Tools;

namespace Roslyn.Grammars
{
    public abstract class Grammar
    {
        public Func<IEnumerableReader<Cell>, Expression> Build { get; set; }

        public Expression this[string code] => Build(new EnumerableReader<Cell>(Lexer.Scan(code)));
    }
}
