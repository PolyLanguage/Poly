using PolyToolkit.Parsing.Ast;

namespace PolyToolkit
{
    public sealed class NonComputedClass : INonComputed
    {
        public ClassNode Node { get; }

        public NonComputedClass(ClassNode node)
        {
            Node = node;
        }
    }
}
