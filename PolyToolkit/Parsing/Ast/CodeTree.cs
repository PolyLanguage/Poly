using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolyToolkit.Parsing.Ast
{
    /// <summary>
    /// Contains ast of code
    /// </summary>
    public sealed class CodeTree : BlockNode
    {
        public override List<AstNode> Childs { get; set; }

        /// <summary>
        /// Create code tree from parser instance
        /// </summary>
        /// <param name="fromparser"></param>
        public CodeTree(PolyParser fromparser) : base(null, 0)
        {
            Childs = fromparser.ParseCode().Childs;
        }
        /// <summary>
        /// Create code tree from list of nodes
        /// </summary>
        /// <param name="tree"></param>
        public CodeTree(List<AstNode> tree) : base(null, 0)
        {
            Childs = tree;
        }

        #region Print
        /// <summary>
        /// Print AST to the console
        /// </summary>
        public void PrintAst()
        {
            foreach(AstNode node in Childs)
            {
                PrintAstNode(node,0);
            }
        }
        private void PrintAstNode(AstNode node, int lvl)
        {
            //add spaces from lvl
            StringBuilder lvlstr = new StringBuilder("");
            if (lvl != 0)
                for (int ls = 0; ls <= lvl; ls++)
                    lvlstr.Append("|  ");

            lvlstr.Append("-");

            string content = "";
            //value
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
            //stmt
            else if(node is VarDeclarationStmtNode)
                content = "(" + ((VarDeclarationStmtNode)node).VarName + ")";
            else if (node is VarAssignStmtNode)
                content = "(" + ((VarAssignStmtNode)node).VarName + ")";

            //print current node
            if (node != null)
            {
                Console.WriteLine(lvlstr.ToString() + node.ToString().Replace("PolyToolkit.Parsing.Ast.", "") + content);
                //print childs
                foreach (AstNode child in node.Childs)
                    PrintAstNode(child, lvl + 1);
            }
        }

        public void PrintScope()
        {
            foreach(AstNode child in Childs)
            {
                if(child is ClassNode)
                    PrintScopeNode(child, 0);
            }
        }
        private void PrintScopeNode(AstNode node,int lvl)
        {
            //add spaces from lvl
            string lvlstr = "";
            for (int ls = 0; ls <= lvl; ls++)
                lvlstr += "|  ";
            lvlstr += "-";

            string content = "";
            List<string> content_after = new List<string>();
            bool container = false;
            if(node is ClassNode)
            {
                container = true;
                content = "class " + ((ClassNode)node).ClassName;
            }
            else if(node is ClassCtorNode)
            {
                container = true;
                content = "ctor (" + string.Join(",",((ClassCtorNode)node).CtorArgs.Values) + ")";
            }
            else if (node is MethodNode)
            {
                MethodNode mnode = (MethodNode)node;
                container = true;
                content = mnode.MethodReturnType.ToString() + " " + mnode.MethodName + " (" + string.Join(",", mnode.MethodArgs.Values) + ")";


                string lvlstrm = "";
                for (int ls = 0; ls <= lvl; ls++)
                    lvlstrm += "|  ";
                lvlstrm += "|  -";

                var vals = mnode.MethodArgs.Values.ToList();
                var keys = mnode.MethodArgs.Keys.ToList();
                for (int i = 0;i < vals.Count;i++)
                {
                    content_after.Add(lvlstrm + "arg " + vals[i] + " " + keys[i]);
                }
            }
            if (node is VarDeclarationStmtNode)
            {
                VarDeclarationStmtNode vnode = (VarDeclarationStmtNode)node;
                container = true;
                content = vnode.VarType.ToString() + " " + vnode.VarName;
            }

            //print current node
            if (content != "")
            {
                Console.WriteLine(lvlstr + content);
                //content after
                foreach(string c_after in content_after)
                    Console.WriteLine(c_after);

                if (container)
                {
                    //print childs
                    foreach (AstNode child in node.Childs)
                        PrintScopeNode(child, lvl + 1);
                }
            }
        }
        #endregion

        public override bool IsAllowed<T>()
        {
            if (AstExtensions.IsAllowedInGlobal<T>())
                return true;
            else
                return true;
        }
    }
}
