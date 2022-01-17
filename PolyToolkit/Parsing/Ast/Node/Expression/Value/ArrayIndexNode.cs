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
        /// Name of the array to get
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Index of item in array
        /// </summary>
        public ExpressionNode Index { get; }

        public ArrayIndexNode(AstNode parent, string varname, PolyType arrayType, ExpressionNode index, int line) : base(parent, line)
        {
            Name = varname;
            Type = arrayType;
            Index = index;
        }
    }
}
