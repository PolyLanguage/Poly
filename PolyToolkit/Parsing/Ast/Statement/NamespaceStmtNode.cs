using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: namespace "hello";
    /// </summary>
    public sealed class NamespaceStmtNode : AstNode
    {
        public string NamespaceValue { get; set; }

        public NamespaceStmtNode(AstNode parent, int line) : base(parent, line) { }
    }
}
