namespace PolyToolkit.Parsing.Ast
{
    public class SubtractExpressionNode : ArithmeticExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Minus; } }

        public SubtractExpressionNode(AstNode parent, int line)
            : base (parent, line)
        { }
    }
}
