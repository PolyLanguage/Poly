using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: "a"
    /// </summary>
    public class StringLiteralNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public PolyType Type { get { return PolyType.StringType; } }
        public string Value { get; }

        public StringLiteralNode(IAstNode parent, string value)
        {
            Parent = parent;

            Value = value;
        }
    }
}
