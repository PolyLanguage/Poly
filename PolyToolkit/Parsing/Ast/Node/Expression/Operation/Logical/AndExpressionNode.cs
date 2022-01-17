namespace PolyToolkit.Parsing.Ast
{
    public sealed class AndExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.And; } }

        public AndExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }
    }
}
