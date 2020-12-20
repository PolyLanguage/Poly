using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public abstract class BinaryBooleanExpression : BinaryExpression
    {
        public BinaryBooleanExpression(IExpression left, IExpression right)
            : base(left, right)
        {
            this.Type = PolyType.BooleanType;
        }
    }
}
