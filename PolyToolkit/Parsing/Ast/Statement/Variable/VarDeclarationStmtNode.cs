using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string a = 200+300;
    /// </summary>
    public class VarDeclarationStmtNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { VarValue }; } }

        public string VarName { get; set; }
        public PolyType VarType { get; set; }
        public IExpressionNode VarValue { get; set; }

        public VarDeclarationStmtNode(IAstNode parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Check if declaration type equals value type
        /// </summary>
        /// <returns></returns>
        public bool IsTypesValid()
        {
            return VarType == VarValue.Type;
        }
    }
}
