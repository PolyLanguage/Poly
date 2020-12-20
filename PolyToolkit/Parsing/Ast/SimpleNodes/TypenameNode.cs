using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string/int/real/object/bool
    /// </summary>
    public class TypenameNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }
        public PolyType Type { get; }

        public TypenameNode(IAstNode parent, PolyType type)
        {
            Type = type;
            Parent = parent;
        }
    }
}
