namespace PolyToolkit.Parsing.Ast
{
    public sealed class LessThanExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Less; } }

        public LessThanExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }

    public sealed class LessThanOrEqualsExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.LessOrEquals; } }

        public LessThanOrEqualsExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }
}
