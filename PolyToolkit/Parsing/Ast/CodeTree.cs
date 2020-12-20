using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Contains ast of code
    /// </summary>
    public class CodeTree : IWithBody
    {
        public IAstNode Parent { get { return null; } set { throw new InvalidOperationException("Parent cannot be set"); } }

        public List<IAstNode> Childs { get { return new List<IAstNode>() { Body }; } }
        public BodyNode Body { get; set; }

        public CodeTree(PolyParser fromparser)
        {
            Body = new BodyNode(this,fromparser.ParseCode().Body.Childs);
        }
        public CodeTree(List<IAstNode> tree)
        {
            Body = new BodyNode(this,tree);
        }

        public void Print()
        {
            foreach(IAstNode node in Childs)
            {
                PrintNode(node,0);
            }
        }
        private void PrintNode(IAstNode node,int lvl)
        {
            //add spaces from lvl
            StringBuilder lvlstr = new StringBuilder("");
            if (lvl != 0)
                for (int ls = 0; ls <= lvl; ls++)
                    lvlstr.Append("|  ");

            lvlstr.Append("-");

            //content
            string content = "";
            if (node is StringLiteralNode)
                content = "(" + ((StringLiteralNode)node).Value + ")";
            else if (node is IntLiteralNode)
                content = "(" + ((IntLiteralNode)node).Value + ")";
            else if (node is RealLiteralNode)
                content = "(" + ((RealLiteralNode)node).Value + ")";
            else if (node is BoolLiteralNode)
                content = "(" + ((BoolLiteralNode)node).Value + ")";
            else if (node is NullLiteralNode)
                content = "(null)";

            //print current node
            if (node != null)
            {
                Console.WriteLine(lvlstr.ToString() + node.ToString().Replace("PolyToolkit.Parsing.Ast.", "") + content);
                //print childs
                foreach (IAstNode child in node.Childs)
                    PrintNode(child, lvl + 1);
            }
        }

        public bool IsAllowed<T>() where T : IAstNode
        {
            if (AstExtensions.IsAllowedInNs<T>())
                return true;
            else
                return true;
        }
    }
}
