using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: "hello"
    /// </summary>
    public class StaticStringNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public string Value { get; }

        public StaticStringNode(IAstNode parent, string val)
        {
            Value = val;
            Parent = parent;
        }
    }
}
