using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    public class EndOfBlockNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return null; } }

        public EndOfBlockNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
