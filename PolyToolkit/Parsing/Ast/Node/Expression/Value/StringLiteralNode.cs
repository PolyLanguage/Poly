using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: "a"
    /// </summary>
    public sealed class StringLiteralNode : ExpressionNode
    {
        public string Value { get; }

        public StringLiteralNode(AstNode parent, string value, int line) : base(parent, line)
        {
            Type = PolyType.StringType;
            Value = value;
        }
    }
}
