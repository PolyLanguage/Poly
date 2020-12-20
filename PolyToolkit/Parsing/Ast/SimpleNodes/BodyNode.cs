using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    public class BodyNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get; }

        public BodyNode(IAstNode parent,List<IAstNode> childs)
        {
            Childs = childs;
            Parent = parent;
        }
    }
}
