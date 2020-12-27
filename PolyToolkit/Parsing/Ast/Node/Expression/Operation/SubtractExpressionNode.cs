using System;

namespace PolyToolkit.Parsing.Ast
{
    public class SubtractExpressionNode : ArithmeticExpressionNode
    {
        public SubtractExpressionNode(IAstNode parent)
            : base (parent)
        { }

        /*public override object Apply(object leftvalue, object rightvalue)
        {
            if (leftvalue is int)
                if (rightvalue is int)
                    return (int)leftvalue - (int)rightvalue;
                else
                    return (int)leftvalue - (double)rightvalue;
            else if (rightvalue is int)
                return (double)leftvalue - (int)rightvalue;
            else
                return (double)leftvalue - (double)rightvalue;
        }*/
    }
}
