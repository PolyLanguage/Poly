namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: null
    /// </summary>
    public sealed class NullLiteralNode : ExpressionNode
    {
        public object Value { get { return null; } }

        public NullLiteralNode(AstNode parent, int line) : base(parent, line)
        {
            Type = PolyTypes.Null; //TODO: other type to be assignable to other types
        }
    }
}
