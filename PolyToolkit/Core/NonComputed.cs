using PolyToolkit.Parsing.Ast;
namespace PolyToolkit
{
    public struct NonComputed
    {
        public AstNode Node { get; }

        public NonComputed(AstNode node)
        {
            Node = node;
        }
    }
}
