using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: true
    /// </summary>
    public sealed class BoolLiteralNode : ExpressionNode
    {
        public bool Value { get; }

        public BoolLiteralNode(AstNode parent, bool value, int line) : base(parent, line)
        {
            Type = PolyTypes.Bool;
            Value = value;
        }
    }
}
