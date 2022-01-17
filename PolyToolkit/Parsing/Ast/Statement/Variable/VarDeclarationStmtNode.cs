using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string a = 200+300;
    /// </summary>
    public sealed class VarDeclarationStmtNode : AstNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>() { VarValue }; set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        public string VarName { get; set; }
        public bool IsConstant { get; set; } = false;

        public PolyType VarType { get; set; }
        public ExpressionNode VarValue { get; set; }

        public VarDeclarationStmtNode(AstNode parent, int line) : base(parent, line) { }

        /// <summary>
        /// Check if declaration type equals value type
        /// </summary>
        /// <returns></returns>
        public bool IsTypesValid()
        {
            if (VarValue == null)
                return true;
            else
                return VarType == VarValue.Type;
        }
    }
}
