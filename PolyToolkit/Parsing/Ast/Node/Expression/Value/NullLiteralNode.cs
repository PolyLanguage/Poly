using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: null
    /// </summary>
    public class NullLiteralNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public PolyType Type { get { return PolyType.ObjectType; } }
        public object Value { get { return null; } }

        public NullLiteralNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
