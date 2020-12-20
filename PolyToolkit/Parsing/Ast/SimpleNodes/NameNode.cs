using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: somename
    /// </summary>
    public class NameNode : IAstNode,IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        /// <summary>
        /// Name value
        /// </summary>
        public string Value { get; }
        /// <summary>
        /// Name as expression
        /// </summary>
        public IExpression Expression { get { return new VarExpression(Value); } }

        public NameNode(IAstNode parent, string name)
        {
            Value = name;
            Parent = parent;
        }
    }
}
