using System.Collections.Generic;

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
        public bool IsStatic { get; set; } = false;
        public Dictionary<string,PolyType> MethodArgs { get; set; }

        public MethodNode(AstNode parent, int line) : base(parent, line)
        {
            Childs = new List<AstNode>();
        }

        public override bool IsAllowed<T>()
        {
            return AstExtensions.IsAllowedInMethod<T>();
        }
    }
}
