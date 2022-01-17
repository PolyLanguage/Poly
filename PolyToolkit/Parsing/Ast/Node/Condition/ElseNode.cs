using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: else { }
    /// </summary>
    public sealed class ElseNode : BlockNode
    {
        public override List<AstNode> Childs { get; set; }
        public ExpressionNode Condition { get; set; }

        public ElseNode(AstNode parent, int line) : base(parent, line)
        {
            Childs = new List<AstNode>();
        }

        public override bool IsAllowed<T>()
        {
            return AstExtensions.IsAllowedInCondition<T>();
        }
    }
}
