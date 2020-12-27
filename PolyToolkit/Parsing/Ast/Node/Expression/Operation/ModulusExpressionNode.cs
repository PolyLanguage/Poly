using System;

namespace PolyToolkit.Parsing.Ast
{
    public class ModulusExpressionNode : ArithmeticExpressionNode
    {
        public ModulusExpressionNode(IAstNode parent)
            : base(parent)
        { }

        /*public override object Apply(object leftvalue, object rightvalue)
        {
            if (leftvalue is int && rightvalye is int)
                return (int)leftvalue % (int)rightvalue;
            else
                //exception...
        }*/
    }
}
