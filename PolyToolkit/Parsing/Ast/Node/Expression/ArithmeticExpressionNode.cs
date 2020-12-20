using System;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class ArithmeticExpressionNode : BinaryExpressionNode
    {
        public ArithmeticExpressionNode(IAstNode parent, IExpressionNode left, IExpressionNode right)
            : base(parent, left, right)
        {
            //int
            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                this.Type = PolyType.IntType;
            //real
            else if ((left.Type == PolyType.IntType && right.Type == PolyType.RealType) ||
                (left.Type == PolyType.RealType && right.Type == PolyType.IntType) ||
                (left.Type == PolyType.RealType && right.Type == PolyType.RealType))
                this.Type = PolyType.RealType;
            //string
            else if (left.Type == PolyType.StringType || right.Type == PolyType.StringType)
                this.Type = PolyType.StringType;
            //else
            else
                this.Type = PolyType.ObjectType;
        }
    }
}
