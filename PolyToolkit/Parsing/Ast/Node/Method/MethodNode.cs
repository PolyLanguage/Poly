using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Example: void Main(string args) {}
    /// </summary>
    public class MethodNode : IAstNode,IWithBody
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { Body }; } }

        public PolyType MethodReturnType { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string,PolyType> MethodArgs { get; set; }
        public BodyNode Body { get; }

        public MethodNode(IAstNode parent)
        {
            Body = new BodyNode(this, new List<IAstNode>());

            Parent = parent;
        }

        public bool IsAllowed<T>() where T : IAstNode
        {
            if (AstExtensions.IsAllowedInMethod<T>())
                return true;
            else
                return true;
        }
    }
}
