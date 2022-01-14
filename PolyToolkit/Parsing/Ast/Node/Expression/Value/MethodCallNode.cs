using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: a()
    /// </summary>
    public sealed class MethodCallNode : ExpressionNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>(); set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        /// <summary>
        /// Name of the method to call
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Arguments to call method
        /// </summary>
        public List<ExpressionNode> Args { get; set; }

        public MethodCallNode(AstNode parent, string methodname, PolyType resulttype, List<ExpressionNode> args, int line) : base(parent, line)
        {
            Type = resulttype;
            Name = methodname;
            Args = args;
        }
    }
}
