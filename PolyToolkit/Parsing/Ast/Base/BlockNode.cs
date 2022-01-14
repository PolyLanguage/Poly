using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// AST Node that represents node with body 
    /// </summary>
    public abstract class BlockNode : AstNode
    {
        /// <summary>
        /// Check if node is allowed inside this node
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <returns>is allowed</returns>
        public abstract bool IsAllowed<T>() where T : AstNode;

        public BlockNode(AstNode parent, int line) : base(parent, line) { }
    }
}
