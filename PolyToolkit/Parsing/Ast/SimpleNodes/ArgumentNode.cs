using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: string a
    /// </summary>
    public class ArgumentNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { ArgType, ArgName }; } }
        public TypenameNode ArgType { get; set; }
        public NameNode ArgName { get; set; }

        public ArgumentNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
