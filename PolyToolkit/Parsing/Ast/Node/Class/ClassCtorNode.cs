using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: ctor(string name) { }
    /// </summary>
    public sealed class ClassCtorNode : BlockNode
    {
        public override List<AstNode> Childs { get; set; }

        public Dictionary<string,PolyType> CtorArgs { get; set; }

        public ClassCtorNode(AstNode parent, int line) : base(parent, line)
        {
            Childs = new List<AstNode>();
        }

        public override bool IsAllowed<T>() => AstExtensions.IsAllowedInMethod<T>() && typeof(T) != typeof(ReturnStmtNode);
    }
}
