using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Evaluation
{
    public abstract class BinaryExpression : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }
        public PolyType Type { get; protected set; }

        public BinaryExpression(IExpression left,IExpression right)
        {
            Left = left;
            Right = right;
            
            //left & right expressions is int --> this expression is int
            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                Type = PolyType.IntType;
            else if (left.Type == PolyType.BooleanType || right.Type == PolyType.BooleanType)
                Type = PolyType.BooleanType;
            else //else --> this expression is real
                Type = PolyType.RealType;
        }

        public object Evaluate(ICurrentContext context)
        {
            object lvalue = this.Left.Evaluate(context);
            object rvalue = this.Right.Evaluate(context);
            return this.Apply(lvalue, rvalue);
        }
        public abstract object Apply(object leftval, object rightval);
    }
}
