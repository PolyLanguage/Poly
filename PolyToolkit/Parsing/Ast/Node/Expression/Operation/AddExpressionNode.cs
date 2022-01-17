using System;

namespace PolyToolkit.Parsing.Ast
{
    public class AddExpressionNode : ArithmeticExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Plus; } }

        public AddExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }
    }
}
