using System;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class BooleanExpressionNode : BinaryExpressionNode
    {
        public BooleanExpressionNode(IAstNode parent, IExpressionNode left, IExpressionNode right)
            : base(parent, left, right)
        {
            this.Type = PolyType.BooleanType;
        }
    }
}
