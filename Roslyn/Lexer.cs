using System.Collections.Generic;
using System.Linq;
using Roslyn.Lexicons;
using Roslyn.Tools;

namespace Roslyn
{
    public static class Lexer
    {
        private static readonly List<(Lexicon lexicon, CellType type)> Templates = new List<(Lexicon key, CellType value)>();

        static Lexer() => Configure();

        public static List<Cell> Scan(string code)
        {
            var res = new List<Cell>();
            Next(new EnumerableReader<char>(code.ToCharArray()), res);
            return res;
        }

        private static void Next(IEnumerableReader<char> reader, ICollection<Cell> res)
        {
            reader.SkipWhile(char.IsWhiteSpace);
            if (reader.End()) return;
            var length = 0;
            (Lexicon lexicon, CellType type) template = default;
            var remain = new string(reader.AsEnumerable().ToArray());
            foreach (var i in Templates)
            {
                var capture = i.lexicon[remain];
                if (!capture.Result || capture.Length <= length) continue;
                length = capture.Length;
                template = i;
            }

            var value = new string(remain.Take(length).ToArray());
            value = template.type switch
            {
                CellType.Chars => value[1..^1],
                CellType.Typeof => value[7..^1],
                CellType.Char=>value[1..^1],
                _ => value
            };
            res.Add(new Cell(template.type, value));
            Next(reader.Skip(length), res);
        }

        private static void Configure()
        {
            //::关键字
            Templates.Add((LexiconBuilder.Build("if"), CellType.If));
            Templates.Add((LexiconBuilder.Build("var"), CellType.Var));
            Templates.Add((LexiconBuilder.Build("while"), CellType.While));
            Templates.Add((LexiconBuilder.Build("class"), CellType.Class));
            Templates.Add((LexiconBuilder.Build("return"), CellType.Return));
            Templates.Add((LexiconBuilder.Build("function"), CellType.Function));

            //::常量   
            Templates.Add((LexiconBuilder.Build("true"), CellType.True));
            Templates.Add((LexiconBuilder.Build("false"), CellType.False));
            Templates.Add((LexiconBuilder.Build("[0-9]*{[0-9]}"), CellType.Digits));
            Templates.Add((LexiconBuilder.Build("\"#{.}\""), CellType.Chars));
            Templates.Add((LexiconBuilder.Build("\'.\'"), CellType.Char));
            Templates.Add((LexiconBuilder.Build("typeof\\(#{.}\\)"), CellType.Typeof));

            //::变量
            Templates.Add((LexiconBuilder.Build("[A-Z|a-z]*{[A-Z|a-z|0-9|_-_]}"), CellType.Variable));

            //::操作符 
            Templates.Add((LexiconBuilder.Build("=="), CellType.Equal));
            Templates.Add((LexiconBuilder.Build("!="), CellType.NotEqual));
            Templates.Add((LexiconBuilder.Build("<="), CellType.LessEqual));
            Templates.Add((LexiconBuilder.Build(">="), CellType.GreaterEqual));
            Templates.Add((LexiconBuilder.Build("="), CellType.Assign));
            Templates.Add((LexiconBuilder.Build(":"), CellType.Quote));
            Templates.Add((LexiconBuilder.Build(">"), CellType.Greater));
            Templates.Add((LexiconBuilder.Build("<"), CellType.Less));
            Templates.Add((LexiconBuilder.Build("+"), CellType.Add));
            Templates.Add((LexiconBuilder.Build("-"), CellType.Subtract));
            Templates.Add((LexiconBuilder.Build("\\*"), CellType.Multiply));
            Templates.Add((LexiconBuilder.Build("&&"), CellType.And));
            Templates.Add((LexiconBuilder.Build("\\|\\|"), CellType.Or));

            //::分隔符 
            Templates.Add((LexiconBuilder.Build("\\."), CellType.Period));
            Templates.Add((LexiconBuilder.Build(","), CellType.Comma));
            Templates.Add((LexiconBuilder.Build("->"), CellType.Arrow));
            Templates.Add((LexiconBuilder.Build(";"), CellType.Semicolon));
            Templates.Add((LexiconBuilder.Build("\\{"), CellType.L1));
            Templates.Add((LexiconBuilder.Build("\\}"), CellType.R1));
            Templates.Add((LexiconBuilder.Build("\\["), CellType.L2));
            Templates.Add((LexiconBuilder.Build("\\]"), CellType.R2));
            Templates.Add((LexiconBuilder.Build("\\("), CellType.L3));
            Templates.Add((LexiconBuilder.Build("\\)"), CellType.R3));
        }
    }

    public struct Cell
    {
        public CellType Type { get; set; }
        public string Value { get; set; }

        public Cell(CellType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum CellType
    {
        //::关键字
        If,
        Var,
        While,
        Class,
        Return,
        Function,

        //::变量
        Variable,

        //::常量
        True,
        False,
        Typeof,
        Digits,
        Chars,
        Char,

        //::运算符
        Assign,
        Quote,
        Equal,
        Greater,
        Less,
        NotEqual,
        GreaterEqual,
        LessEqual,
        Add,
        Subtract,
        Multiply,
        And,
        Or,

        //::分隔符
        Period,
        Arrow,
        Comma,
        Semicolon,
        L1,
        L2,
        L3,
        R1,
        R2,
        R3,
    }
}
