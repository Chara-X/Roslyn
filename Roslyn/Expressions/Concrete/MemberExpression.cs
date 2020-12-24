using System;
using System.Linq;
using System.Reflection;
using Roslyn.Expressions.Structure;

namespace Roslyn.Expressions.Concrete
{
    public class MemberExpression : UnaryExpression
    {
        public string Name { get; set; }

        internal MemberExpression(Expression sub, string name) : base(sub) => Name = name;

        public static MemberExpression LoadRef(Expression sub, string name)
        {
            var e = new MemberExpression(sub, name);
            e.Value = (i) => e.LoadRef(i);
            e.Refer = e.LoadRef;
            return e;
        }

        public static MemberExpression Load(Expression sub, string name)
        {
            var e = new MemberExpression(sub, name);
            e.Value = e.Load;
            return e;
        }

        private ref object LoadRef(ExpressionContext context)
        {
            var i = (ExpressionContext) Sub.Value(context);
            if (i == null) throw new Exception();
            return ref i.Objects[Name];
        }

        private object Load(ExpressionContext context)
        {
            var i = Sub.Value(context);
            return i switch
            {
                ExpressionContext c => c.Objects[Name],
                Type t => t.GetMember(Name).Cast<MethodInfo>().ToArray(),
                _ => i.GetType().GetMember(Name).Cast<MethodInfo>().ToArray()
            };
        }

        //private object Call(ExpressionContext context)
        //{
        //    var i = Sub.Value(context);
        //    var parameters = (object[])Parameters.Value(context);
        //    return i switch
        //    {
        //        ExpressionContext o => o.Call(Name, parameters),
        //        Type t => t.GetMethod(Name, Typeof(parameters))?.Invoke(null, parameters),
        //        _ => i.GetType().GetMethod(Name, Typeof(parameters))?.Invoke(i, parameters)
        //    };
        //}

        //private static Type[] Typeof(IEnumerable<object> values) => values?.Select(k => k.GetType()).ToArray() ?? Type.EmptyTypes;
    }
}
