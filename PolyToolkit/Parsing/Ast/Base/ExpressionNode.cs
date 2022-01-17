namespace PolyToolkit.Parsing.Ast
{
    public abstract class ExpressionNode : AstNode
    {
        /// <summary>
        /// Type of the expression
        /// </summary>
        public PolyType Type { get; protected set; }

        /// <summary>
        /// Is constant and cannot be changed?
        /// </summary>
        public bool IsConstant { get; set; } = false;

        public ExpressionNode(AstNode parent, int line) : base(parent, line) { }
    }
}
