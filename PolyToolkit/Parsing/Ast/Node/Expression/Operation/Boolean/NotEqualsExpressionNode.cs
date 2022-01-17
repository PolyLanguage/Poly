namespace PolyToolkit.Parsing.Ast
{
    public sealed class NotEqualsExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.NotEquals; } }

        public NotEqualsExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }
}
