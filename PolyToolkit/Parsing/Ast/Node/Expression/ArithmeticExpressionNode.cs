using System;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class ArithmeticExpressionNode : BinaryExpressionNode
    {
        public ArithmeticExpressionNode(AstNode parent, int line)
            : base(parent, line)
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
                else if (Left.Type == PolyType.RealType || Right.Type == PolyType.RealType)
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
