using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: 200300100
    /// </summary>
    public sealed class IntLiteralNode : ExpressionNode
    {
        public int Value { get; }

        public IntLiteralNode(AstNode parent, int value, int line) : base(parent, line)
        {
            Type = PolyType.IntType;
            Value = value;
        }
    }
}
