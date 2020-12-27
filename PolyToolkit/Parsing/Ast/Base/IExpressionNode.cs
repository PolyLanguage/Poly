namespace PolyToolkit.Parsing.Ast
{
    public interface IExpressionNode : IAstNode
    {
        /// <summary>
        /// Type of the expression
        /// </summary>
        PolyType Type { get; }
    }
}
