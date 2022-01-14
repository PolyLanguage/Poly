using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: 1.0200300
    /// </summary>
    public sealed class RealLiteralNode : ExpressionNode
    {
        public double Value { get; }

        public RealLiteralNode(AstNode parent, double value, int line) : base(parent, line)
        {
            Type = PolyType.RealType;
            Value = value;
        }
    }
}
