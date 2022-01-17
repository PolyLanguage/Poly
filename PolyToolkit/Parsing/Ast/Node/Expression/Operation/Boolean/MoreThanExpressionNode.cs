namespace PolyToolkit.Parsing.Ast
{
    public sealed class MoreThanExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.More; } }

        public MoreThanExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }

    public sealed class MoreThanOrEqualsExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.MoreOrEquals; } }

        public MoreThanOrEqualsExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }
}
