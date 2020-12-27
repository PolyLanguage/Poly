using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: return 200 + x;
    /// </summary>
    public class ReturnStmtNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { ReturnValue }; } }

        public IExpressionNode ReturnValue { get; set; }

        public ReturnStmtNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
