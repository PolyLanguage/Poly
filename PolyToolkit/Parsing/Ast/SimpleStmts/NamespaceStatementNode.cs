using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    public class NamespaceStatementNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { NamespaceValue }; } }

        public StaticStringNode NamespaceValue { get; set; }

        public NamespaceStatementNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
