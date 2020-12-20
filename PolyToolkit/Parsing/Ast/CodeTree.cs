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
            string lvlstr = "";
            if (lvl != 0)
                for (int ls = 0; ls <= lvl; ls++)
                    lvlstr += "|  ";

            lvlstr += "-";

            //content
            string content = "";
            if (node is TypenameNode)
                content = "(" + ((TypenameNode)node).Type.Name + ")";
            else if (node is NameNode)
                content = "(" + ((NameNode)node).Value + ")";
            else if (node is ExpressionNode)
            {
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                object evalRes = ((ExpressionNode)node).Expression.Evaluate(new Evaluation.DefaultContext());
                timer.Stop();
                content = "(" + evalRes + ":"+timer.ElapsedMilliseconds+"ms)";
            }

            //print current node
            Console.WriteLine(lvlstr+node.ToString().Replace("PolyToolkit.Parsing.Ast.","")+content);
            //print childs
            foreach (IAstNode child in node.Childs)
                PrintNode(child, lvl + 1);
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
