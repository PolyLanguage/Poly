using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public class EqualExpression : BinaryBooleanExpression
    {
        private Func<object, object, bool> apply;

        public EqualExpression(IExpression left, IExpression right)
            : base(left, right)
        {
            //numbers
            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                this.apply = (a, b) => (int)a == (int)b;
            else if (left.Type == PolyType.RealType && right.Type == PolyType.RealType)
                this.apply = (a, b) => (double)a == (double)b;
            else if (left.Type == PolyType.IntType && right.Type == PolyType.RealType)
                this.apply = (a, b) => (int)a == (double)b;
            else if (left.Type == PolyType.RealType && right.Type == PolyType.IntType)
                this.apply = (a, b) => (double)a == (int)b;

            //boolean
            else if (left.Type == PolyType.BooleanType && right.Type == PolyType.BooleanType)
                this.apply = (a, b) => (bool)a == (bool)b;
        }

        public override object Apply(object leftvalue, object rightvalue)
        {
            if (this.apply != null)
                return this.apply(leftvalue, rightvalue);

            return leftvalue == rightvalue;
        }
    }
}
