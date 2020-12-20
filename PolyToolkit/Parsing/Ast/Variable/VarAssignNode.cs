using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string a = 200+300;
    /// </summary>
    public class VarAssignNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { VarName, VarValue }; } }

        public NameNode VarName { get; set; }
        public IExpressionNode VarValue { get; set; }
        public PolyType VarValueType { get { return VarValue.Expression.Type; } }

        /// <summary>
        /// Throws exception if type mismatch
        /// </summary>
        /// <param name="type"></param>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public VarAssignNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
