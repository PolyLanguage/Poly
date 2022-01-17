namespace PolyToolkit.Parsing.Ast
{
    public class MultiplyExpressionNode : ArithmeticExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Multiply; } }

        public MultiplyExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }
    }
}
