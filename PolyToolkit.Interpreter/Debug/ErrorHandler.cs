using System;

using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Interpreter
{
    public class ErrorHandler
    {
        public ErrorHandler()
        { }

        public void ReportError(AstNode node, string message)
        {
            Console.WriteLine($"{message} at line {node?.Line}");
        }
    }
}
