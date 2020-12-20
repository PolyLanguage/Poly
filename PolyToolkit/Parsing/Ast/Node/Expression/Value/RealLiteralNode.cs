using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: 1.0200300
    /// </summary>
    public class RealLiteralNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public PolyType Type { get { return PolyType.RealType; } }
        public double Value { get; }

        public RealLiteralNode(IAstNode parent, double value)
        {
            Parent = parent;

            Value = value;
        }
    }
}
