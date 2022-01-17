namespace PolyToolkit.Parsing.Ast
{
    public sealed class OrExpressionNode : BooleanExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Or; } }

        public OrExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }
    }
}
