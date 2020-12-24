using System;

namespace Roslyn.Expressions.Structure
{
    public class BinaryExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        internal BinaryExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public static BinaryExpression Assign(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = (i) => e.Refer(i);
            e.Refer = e.Assign;
            return e;
        }

        public static BinaryExpression Quote(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = (i) => e.Refer(i);
            e.Refer = e.Quote;
            return e;
        }

        public static BinaryExpression Or(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Or;
            return e;
        }

        public static BinaryExpression And(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.And;
            return e;
        }

        public static BinaryExpression Equal(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Equal;
            return e;
        }

        public static BinaryExpression NotEqual(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.NotEqual;
            return e;
        }

        public static BinaryExpression Greater(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Greater;
            return e;
        }

        public static BinaryExpression Less(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Less;
            return e;
        }

        public static BinaryExpression GreaterEqual(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.GreaterEqual;
            return e;
        }

        public static BinaryExpression LessEqual(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.LessEqual;
            return e;
        }

        public static BinaryExpression Add(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Add;
            return e;
        }

        public static BinaryExpression Subtract(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Subtract;
            return e;
        }

        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            var e = new BinaryExpression(left, right);
            e.Value = e.Multiply;
            return e;
        }

        private ref object Assign(ExpressionContext context)
        {
            ref var i = ref Left.Refer(context);
            var j = Right.Value(context);
            i = j;
            return ref i;
        }

        private ref object Quote(ExpressionContext context)
        {
            ref var i = ref Left.Refer(context);
            var j = Right;
            i = j;
            return ref i;
        }

        private object Or(ExpressionContext context)
        {
            var i = (bool) Left.Value(context);
            if (i) return true;
            var j = (bool) Right.Value(context);
            return j;
        }

        private object And(ExpressionContext context)
        {
            var i = (bool) Left.Value(context);
            if (!i) return false;
            var j = (bool) Right.Value(context);
            return j;
        }

        private object Equal(ExpressionContext context)
        {
            var i = Left.Value(context);
            var j = Right.Value(context);
            return i.Equals(j);
        }

        private object NotEqual(ExpressionContext context)
        {
            var i = Left.Value(context);
            var j = Right.Value(context);
            return !i.Equals(j);
        }

        private object Greater(ExpressionContext context)
        {
            var i = (IComparable) Left.Value(context);
            var j = (IComparable) Right.Value(context);
            return i.CompareTo(j) == 1;
        }

        private object Less(ExpressionContext context)
        {
            var i = (IComparable)Left.Value(context);
            var j = (IComparable)Right.Value(context);
            return i.CompareTo(j) == -1;
        }

        private object GreaterEqual(ExpressionContext context)
        {
            var i = (IComparable) Left.Value(context);
            var j = (IComparable) Right.Value(context);
            var res = i.CompareTo(j);
            return res == 1 || res == 0;
        }

        private object LessEqual(ExpressionContext context)
        {
            var i = (IComparable) Left.Value(context);
            var j = (IComparable) Right.Value(context);
            var res = i.CompareTo(j);
            return res == -1 || res == 0;
        }

        private object Add(ExpressionContext context)
        {
            var i = Left.Value(context);
            var j = Right.Value(context);
            return i switch
            {
                int a when j is int b => a + b,
                string a when j is string b => a + b,
                _ => throw new Exception()
            };
        }

        private object Subtract(ExpressionContext context)
        {
            var i = Left.Value(context);
            var j = Right.Value(context);
            return i switch
            {
                int a when j is int b => a - b,
                _ => throw new Exception()
            };
        }

        private object Multiply(ExpressionContext context)
        {
            var i = Left.Value(context);
            var j = Right.Value(context);
            return i switch
            {
                int a when j is int b => a * b,
                int a when j is double b => a * b,
                double a when j is int b => a * b,
                double a when j is double b => a * b,
                _ => throw new Exception()
            };
        }
    }
}
