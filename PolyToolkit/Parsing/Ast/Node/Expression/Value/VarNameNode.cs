using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: a
    /// </summary>
    public class VarNameNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>(); } }

        public PolyType Type { get; }
        public string Name { get; }

        public VarNameNode(IAstNode parent, string varname, PolyType vartype)
        {
            Parent = parent;

            Type = vartype;
            Name = varname;
        }
    }
}
