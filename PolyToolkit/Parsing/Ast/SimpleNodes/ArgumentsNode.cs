using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    public class ArgumentsNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return Args.ToIAstList<ArgumentNode>(); } }
        public List<ArgumentNode> Args { get; set; }

        public ArgumentsNode(IAstNode parent)
        {
            Args = new List<ArgumentNode>();
            Parent = parent;
        }
    }
}
