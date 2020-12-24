using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Roslyn.Expressions;
using Roslyn.Expressions.Concrete;

namespace Roslyn.Shared
{
    public class Assembly
    {
        private static readonly IDictionary<string, ClassExpression> Types = new Dictionary<string, ClassExpression>();

        public static object GetType(string name)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(i => i.GetType(name) != null);
            var type = assembly?.GetType(name);
            if (type != null)
                return type;
            return Types[name];
        }

        public static void Manifest(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                using var reader = new StreamReader(file);
                var type = (ClassExpression) ExpressionBuilder.Build(reader.ReadToEnd());
                Types.Add(type.Name, type);
            }
        }

        public static void Mirror(params Type[] types)
        {
            foreach (var type in types)
                System.Reflection.Assembly.GetAssembly(type);
        }

        public static object Run() =>
            ((FunctionExpression) ((ExpressionContext) ((ClassExpression) GetType("StartUp")).Value(null))
                ["Main"]).Invoke(null);
    }
}