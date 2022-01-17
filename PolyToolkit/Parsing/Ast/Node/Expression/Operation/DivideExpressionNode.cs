using System;

namespace PolyToolkit.Parsing.Ast
{
    public class DivideExpressionNode : ArithmeticExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Divide; } }

        public DivideExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }
    }
}
