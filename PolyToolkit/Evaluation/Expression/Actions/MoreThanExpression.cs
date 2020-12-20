using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Evaluation
{
    public class MoreThanExpression : BinaryBooleanExpression
    {
        private Func<object, object, bool> apply;

        public MoreThanExpression(IExpression left, IExpression right)
            : base(left, right)
        {
            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                this.apply = (a, b) => (int)a > (int)b;
            else if (left.Type == PolyType.RealType && right.Type == PolyType.RealType)
                this.apply = (a, b) => (double)a > (double)b;
            else if (left.Type == PolyType.IntType && right.Type == PolyType.RealType)
                this.apply = (a, b) => (int)a > (double)b;
            else if (left.Type == PolyType.RealType && right.Type == PolyType.IntType)
                this.apply = (a, b) => (double)a > (int)b;
        }

        public override object Apply(object leftvalue, object rightvalue)
        {
            if (this.apply != null)
                return this.apply(leftvalue, rightvalue);

            return null;
        }
    }

    public class MoreThanOrEqualsExpression : BinaryBooleanExpression
    {
        private Func<object, object, bool> apply;

        public MoreThanOrEqualsExpression(IExpression left, IExpression right)
            : base(left, right)
        {
            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                this.apply = (a, b) => (int)a >= (int)b;
            else if (left.Type == PolyType.RealType && right.Type == PolyType.RealType)
                this.apply = (a, b) => (double)a >= (double)b;
            else if (left.Type == PolyType.IntType && right.Type == PolyType.RealType)
                this.apply = (a, b) => (int)a >= (double)b;
            else if (left.Type == PolyType.RealType && right.Type == PolyType.IntType)
                this.apply = (a, b) => (double)a >= (int)b;
        }

        public override object Apply(object leftvalue, object rightvalue)
        {
            if (this.apply != null)
                return this.apply(leftvalue, rightvalue);

            return null;
        }
    }
}
