using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Roslyn.Expressions.Structure;

namespace Roslyn.Expressions.Concrete
{
    public class ExecuteExpression : SuffixExpression
    {
        public Expression[] Parameters { get; set; }

        internal ExecuteExpression(Expression sub, Expression[] parameters) : base(sub) => Parameters = parameters;

        public static ExecuteExpression Exec(Expression sub, Expression[] parameters)
        {
            var e = new ExecuteExpression(sub, parameters);
            e.Value = e.Exec;
            return e;
        }

        private object Exec(ExpressionContext context)
        {
            var i = Sub.Value(context);
            var parameters = Parameters.Select(j=>j.Value(context)).ToArray();
            switch (i)
            {
                case MethodInfo[] ms:
                {
                    var types = Typeof(parameters);
                    i = ms.First(j =>
                        j.IsStatic && HasTypes(j, types) ||
                        !j.IsStatic && HasTypes(j, types.Skip(1).ToArray()));
                    break;
                }
            }

            return i switch
            {
                Type t => Activator.CreateInstance(t, parameters),
                ClassExpression c => c.Value(null),
                FunctionExpression f => f.Invoke(parameters),
                Expression e => e.Value(context),
                MethodInfo m => m.IsStatic
                    ? m.Invoke(null, parameters)
                    : m.Invoke(parameters[0], parameters.Skip(1).ToArray()),
                _ => throw new Exception("error：formula of [" + i + "] can't execute")
            };
        }

        private static bool HasTypes(MethodBase method, IReadOnlyList<Type> types)
        {
            var ps = method.GetParameters();
            if (ps.Length != types.Count) return false;
            return !ps.Select(t => t.ParameterType).Where((type, i) => !type.IsAssignableFrom(types[i])).Any();
        }

        private static Type[] Typeof(IEnumerable<object> values) => values?.Select(k => k.GetType()).ToArray() ?? Type.EmptyTypes;
    }
}
