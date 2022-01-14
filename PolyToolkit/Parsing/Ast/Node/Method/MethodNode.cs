using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: method Main() { }
    /// </summary>
    public sealed class MethodNode : BlockNode
    {
        public override List<AstNode> Childs { get; set; }

        public PolyType MethodReturnType { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string,PolyType> MethodArgs { get; set; }

        public MethodNode(AstNode parent, int line) : base(parent, line)
        {
            Childs = new List<AstNode>();
        }

        public override bool IsAllowed<T>()
        {
            if (AstExtensions.IsAllowedInMethod<T>())
                return true;
            else
                return false;
        }
    }
}
