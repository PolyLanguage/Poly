using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: return 200 + x;
    /// </summary>
    public sealed class ReturnStmtNode : AstNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>() { ReturnValue }; set => throw new InvalidOperationException("Childs of this node cannot be set"); }
        public ExpressionNode ReturnValue { get; set; }

        public ReturnStmtNode(AstNode parent, int line) : base(parent, line) { }
    }
}
