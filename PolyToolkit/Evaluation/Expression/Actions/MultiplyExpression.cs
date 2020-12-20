using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public class MultiplyExpression : BinaryArithmeticExpression
    {
        public MultiplyExpression(IExpression left, IExpression right)
            : base(left, right)
        {
            if (left.Type == PolyType.StringType && right.Type == PolyType.IntType ||
                left.Type == PolyType.IntType && right.Type == PolyType.StringType)
                this.Type = PolyType.StringType;

            
        }

        public override object Apply(object leftvalue, object rightvalue)
        {
            if (leftvalue is string && rightvalue is int)
                return ((string)leftvalue).Multiply((int)rightvalue);
            else if (leftvalue is int && rightvalue is string)
                return ((string)rightvalue).Multiply((int)leftvalue);

            if (leftvalue is int)
                if (rightvalue is int)
                    return (int)leftvalue * (int)rightvalue;
                else
                    return (int)leftvalue * (double)rightvalue;
            else if (rightvalue is int)
                return (double)leftvalue * (int)rightvalue;
            else
                return (double)leftvalue * (double)rightvalue;
        }
    }
}
