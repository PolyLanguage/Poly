using PolyToolkit.Parsing.Ast;

namespace PolyToolkit
{
    public sealed class NonComputedMethod : INonComputed
    {
        public MethodNode Node { get; }

        public NonComputedMethod(MethodNode node)
        {
            Node = node;
        }
    }
}
