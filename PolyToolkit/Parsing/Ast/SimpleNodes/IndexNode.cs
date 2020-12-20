using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: [0]
    /// </summary>
    public class IndexNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { Index }; } }

        public IExpressionNode Index { get; set; }

        public IndexNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
