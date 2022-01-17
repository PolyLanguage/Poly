namespace PolyToolkit.Parsing.Ast
{
    public class ModulusExpressionNode : ArithmeticExpressionNode
    {
        public override MathOperation Op { get { return MathOperation.Modulus; } }

        public ModulusExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }
    }
}
