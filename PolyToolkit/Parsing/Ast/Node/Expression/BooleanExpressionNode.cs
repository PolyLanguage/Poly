namespace PolyToolkit.Parsing.Ast
{
    public abstract class BooleanExpressionNode : BinaryExpressionNode
    {
        public BooleanExpressionNode(AstNode parent, int line)
            : base(parent, line)
        {
            Type = PolyTypes.Bool;
        }
    }
}
