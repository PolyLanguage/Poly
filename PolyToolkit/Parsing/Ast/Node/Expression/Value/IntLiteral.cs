using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: 200300100
    /// </summary>
    public class IntLiteralNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public PolyType Type { get { return PolyType.IntType; } }
        public int Value { get; }

        public IntLiteralNode(IAstNode parent, int value)
        {
            Parent = parent;

            Value = value;
        }
    }
}
