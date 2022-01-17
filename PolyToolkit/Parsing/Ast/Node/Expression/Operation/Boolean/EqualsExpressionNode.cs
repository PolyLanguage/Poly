namespace PolyToolkit.Parsing.Ast
{
    public sealed class EqualsExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Equals; } }

        public EqualsExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }
}
