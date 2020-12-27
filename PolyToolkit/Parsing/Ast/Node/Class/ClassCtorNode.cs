using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    // classname(<ARGS>) { <BODY> }
    public class ClassCtorNode : IAstNode,IWithBody
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get; set; }

        public Dictionary<string,PolyType> CtorArgs { get; set; }

        public ClassCtorNode(IAstNode parent)
        {
            Parent = parent;
            Childs = new List<IAstNode>();
        }

        public bool IsAllowed<T>() where T : IAstNode
        {
            if (AstExtensions.IsAllowedInMethod<T>())
                return true;
            else
                return false;
        }
    }
}
