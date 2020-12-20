using System;
using System.Collections.Generic;
using System.Text;

using PolyToolkit;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
using PolyToolkit.Compilation;
namespace PolyToolkit.Evaluation
{
    public class CodeEvaluator
    {
        public PolyProgram Program { get; set; }

        public CodeEvaluator(PolyProgram program)
        {
            Program = program;
        }

        public void EvaluateProgram(string pathToMethod)
        {
            //TODO: evaluate
        }
    }
}
