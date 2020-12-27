using System;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class BooleanExpressionNode : BinaryExpressionNode
    {
        public BooleanExpressionNode(IAstNode parent)
            : base(parent)
        {
            this.Type = PolyType.BooleanType;
        }
    }
}
