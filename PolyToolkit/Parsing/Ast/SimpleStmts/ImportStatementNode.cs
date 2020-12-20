using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: import: "System";
    /// </summary>
    public class ImportStatementNode : IAstNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { ImportValue }; } }
        public StaticStringNode ImportValue { get; set; }

        public ImportStatementNode(IAstNode parent)
        {
            Parent = parent;
        }
    }
}
