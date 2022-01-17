using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string a = 200+300;
    /// </summary>
    public sealed class VarAssignStmtNode : AstNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>() { VarValue }; set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        public string VarName { get; set; }
        public ExpressionNode VarValue { get; set; }

        public VarAssignStmtNode(AstNode parent, int line) : base(parent, line) { }

        /// <summary>
        /// Check if declaration type equals value type
        /// </summary>
        /// <returns></returns>
        public bool IsTypesValid()
        {
            if (VarValue != null)
                return Parent.GetNameType(VarName) == VarValue.Type;
            else
                return false;
        }
    }
}
