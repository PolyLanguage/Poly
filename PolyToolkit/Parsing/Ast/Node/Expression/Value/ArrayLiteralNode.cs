namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: [1,2,3]
    /// </summary>
    public sealed class ArrayLiteralNode : ExpressionNode
    {
        public ExpressionNode[] Values { get; }

        public ArrayLiteralNode(AstNode parent, PolyType valuesType, ExpressionNode[] values, int line) : base(parent, line)
        {
            Type = new PolyTypeArray();
            Type.Of(valuesType);

            Values = values;
        }
    }
}
