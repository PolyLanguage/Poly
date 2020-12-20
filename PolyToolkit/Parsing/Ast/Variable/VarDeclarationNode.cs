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
    public class VarDeclarationNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { VarType, VarName, VarValue }; } }

        public TypenameNode VarType { get; set; }
        public NameNode VarName { get; set; }
        public IExpressionNode VarValue { get; set; }
        /// <summary>
        /// Throws exception if type mismatch
        /// </summary>
        /// <param name="type"></param>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public VarDeclarationNode(IAstNode parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Check if declaration type equals value type
        /// </summary>
        /// <returns></returns>
        public bool IsTypesValid()
        {
            return VarType.Type == VarValue.Expression.Type;
        }
    }
}
