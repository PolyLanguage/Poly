using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public abstract class BinaryArithmeticExpression : BinaryExpression
    {
        public BinaryArithmeticExpression(IExpression left, IExpression right)
                    : base(left, right)
        {
            //same as any binary expression logic

            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                this.Type = PolyType.IntType;
            else
                this.Type = PolyType.RealType;
        }
    }
}
