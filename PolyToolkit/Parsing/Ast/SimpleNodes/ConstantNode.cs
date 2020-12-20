using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// example: 3243
    /// </summary>
    class ConstantNode : IAstNode,IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public object Value { get; }
        public IExpression Expression { get { return new ConstExpression(Value); } }

        public ConstantNode(IAstNode parent, object val)
        {
            Value = val;
            Parent = parent;
        }
    }
}
