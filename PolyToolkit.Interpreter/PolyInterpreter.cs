using System;

using PolyToolkit;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Interpreter
{
    public sealed class PolyInterpreter
    {
        private CodeTree Tree { get; set; }

        public PolyInterpreter(CodeTree tree)
        {
            Tree = tree;
        }

        public void Begin()
        {
            foreach(IAstNode coreNode in Tree.Childs)
            {
                if(coreNode is ClassNode && ((ClassNode)coreNode).ClassName == "Program")
                {
                   //find main method & call it
                }
            }
        }
    }
}
