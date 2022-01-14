namespace PolyToolkit.Parsing.Ast
{
    public abstract class ExpressionNode : AstNode
    {
        /// <summary>
        /// Type of the expression
        /// </summary>
        public PolyType Type { get; protected set; }

        public ExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }
}
