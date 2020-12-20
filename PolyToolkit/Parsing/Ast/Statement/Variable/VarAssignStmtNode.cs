using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string a = 200+300;
    /// </summary>
    public class VarAssignStmtNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { VarValue }; } }

        public string VarName { get; set; }
        public IExpressionNode VarValue { get; set; }

        public VarAssignStmtNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
