using System;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class ArithmeticExpressionNode : BinaryExpressionNode
    {
        public ArithmeticExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }

        protected override void ApplyType()
        {
            base.ApplyType();

            if (Left != null && Right != null && Left.Type == Right.Type)
                Type = Left.Type;
            else
                Type = PolyTypes.Unknown;
        }
    }
}
