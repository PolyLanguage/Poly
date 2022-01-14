using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: a
    /// </summary>
    public sealed class VarNameNode : ExpressionNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>(); set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        /// <summary>
        /// Name of the variable to get
        /// </summary>
        public string Name { get; }

        public VarNameNode(AstNode parent, string varname, PolyType vartype, int line) : base(parent, line)
        {
            Type = vartype;
            Name = varname;
        }
    }
}
