using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: class Car { }
    /// </summary>
    public sealed class ClassNode : BlockNode
    {
        public override List<AstNode> Childs { get; set; }

        public string ClassName { get; set; }

        public ClassNode(AstNode parent, int line) : base(parent, line)
        {
            Childs = new List<AstNode>();
        }

        public override bool IsAllowed<T>() => AstExtensions.IsAllowedInClass<T>();
    }
}
