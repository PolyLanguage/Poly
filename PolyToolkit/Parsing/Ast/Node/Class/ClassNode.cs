using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Can contain only variable & method declarations
    /// </summary>
    public class ClassNode : IAstNode,IWithBody
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { Body }; } }

        public string ClassName { get; set; }
        public BodyNode Body { get; }

        public ClassNode(IAstNode parent)
        {
            Body = new BodyNode(this, new List<IAstNode>());
            Parent = parent;
        }

        public bool IsAllowed<T>() where T : IAstNode
        {
            if (AstExtensions.IsAllowedInClass<T>())
                return true;
            else
                return true;
        }
    }
}
