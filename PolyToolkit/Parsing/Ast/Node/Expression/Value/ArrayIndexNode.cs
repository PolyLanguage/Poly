using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: arr[0]
    /// </summary>
    public sealed class ArrayIndexNode : ExpressionNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>(); set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        /// <summary>
        /// Array value
        /// </summary>
        public ExpressionNode Array { get; }
        /// <summary>
        /// Index of item in array
        /// </summary>
        public ExpressionNode Index { get; set; }

        public ArrayIndexNode(AstNode parent, ExpressionNode arr, int line) : base(parent, line)
        {
            Array = arr;
            Type = arr.Type.T;
        }
    }
}
