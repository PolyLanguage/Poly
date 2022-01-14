namespace PolyToolkit.Parsing.Ast
{
    public sealed class UnknownNode : AstNode
    {
        public UnknownNode(AstNode parent, int line) : base(parent, line) { }
    }
}
