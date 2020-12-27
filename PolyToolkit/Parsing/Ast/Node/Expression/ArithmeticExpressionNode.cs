using System;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class ArithmeticExpressionNode : BinaryExpressionNode
    {
        public ArithmeticExpressionNode(IAstNode parent)
            : base(parent)
        { }

        public override void ApplyType()
        {
            base.ApplyType();

            if (Left != null && Right != null)
            {
                //int
                if (Left.Type == Right.Type)
                    this.Type = Left.Type;
                //real
                else if ((Left.Type == PolyType.IntType && Right.Type == PolyType.RealType) ||
                    (Left.Type == PolyType.RealType && Right.Type == PolyType.IntType) ||
                    (Left.Type == PolyType.RealType && Right.Type == PolyType.RealType))
                    this.Type = PolyType.RealType;
                //object
                else if (Left.Type == PolyType.ObjectType || Right.Type == PolyType.ObjectType)
                    this.Type = PolyType.ObjectType;
                //else
                else
                    this.Type = PolyType.UnknownType;
            }
        }
    }
}
