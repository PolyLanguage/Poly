using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Parsing.Ast
{
    public class ExpressionNode : IAstNode,IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public IExpression Expression { get; }

        public ExpressionNode(IAstNode parent,IExpression expr)
        {
            Expression = expr;
            Parent = parent;
        }
    }
}
