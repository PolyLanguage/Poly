using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: import: "System";
    /// </summary>
    public sealed class ImportStmtNode : AstNode
    {
        public string ImportValue { get; set; }

        public ImportStmtNode(AstNode parent, int line) : base(parent, line) { }
    }
}
