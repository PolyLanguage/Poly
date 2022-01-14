using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override bool IsAllowed<T>()
        {
            if (AstExtensions.IsAllowedInMethod<T>() && typeof(T) != typeof(ReturnStmtNode))
                return true;
            else
                return false;
        }
    }
}
