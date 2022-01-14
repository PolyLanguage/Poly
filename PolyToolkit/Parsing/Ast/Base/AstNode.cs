using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Represents Node of AST
    /// </summary>
    public abstract class AstNode
    {
        /// <summary>
        /// Tag of node, can be used to store some information
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Position of node in file by line
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Parent node
        /// </summary>
        public AstNode Parent { get; set; }
        /// <summary>
        /// Child nodes
        /// </summary>
        public virtual List<AstNode> Childs { get { return new List<AstNode>(); } set { throw new InvalidOperationException("Childs of this node cannot be set"); } }

        public AstNode(AstNode parent, int line)
        {
            this.Parent = parent;

            this.Line = line;
        }
    }
}
