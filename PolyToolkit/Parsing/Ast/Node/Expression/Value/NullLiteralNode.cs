using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: null
    /// </summary>
    public sealed class NullLiteralNode : ExpressionNode
    {
        public object Value { get { return null; } }

        public NullLiteralNode(AstNode parent, int line) : base(parent, line)
        {
            Type = PolyType.ObjectType;
        }
    }
}
