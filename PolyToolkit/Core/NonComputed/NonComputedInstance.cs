using PolyToolkit.Parsing.Ast;

namespace PolyToolkit
{
    public sealed class NonComputedInstance : INonComputed
    {
        //TODO: use NewInstanceExpressionNode
        public ExpressionNode Node { get; }

        public NonComputedInstance(ExpressionNode node)
        {
            Node = node;
        }
    }
}
