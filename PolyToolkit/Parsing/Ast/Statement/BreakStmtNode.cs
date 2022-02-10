using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: break;
    /// </summary>
    public sealed class BreakStmtNode : AstNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>() {}; set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        public BreakStmtNode(AstNode parent, int line) : base(parent, line) { }
    }
}
