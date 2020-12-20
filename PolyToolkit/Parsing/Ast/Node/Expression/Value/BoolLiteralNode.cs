using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: true
    /// </summary>
    public class BoolLiteralNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public PolyType Type { get { return PolyType.BooleanType; } }
        public bool Value { get; }

        public BoolLiteralNode(IAstNode parent, bool value)
        {
            Parent = parent;

            Value = value;
        }
    }
}
