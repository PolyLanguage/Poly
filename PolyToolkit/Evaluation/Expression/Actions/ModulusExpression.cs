using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public class ModulusExpression : BinaryArithmeticExpression
    {
        public ModulusExpression(IExpression left, IExpression right)
            : base(left, right)
        {
        }

        public override object Apply(object leftvalue, object rightvalue)
        {
            if (leftvalue is int && rightvalue is int)
                return (int)leftvalue % (int)rightvalue;

            //left or right value was not int
            throw new InvalidOperationException("Modulus operation accepts only int values");
        }
    }
}
