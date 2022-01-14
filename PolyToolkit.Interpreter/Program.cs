using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PolyToolkit.Debug;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
using PolyToolkit.Compilation;
namespace PolyToolkit.Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PolyParser CLI";

            Console.WriteLine("Step Log ? (true/sa/-)");
            Console.Write("> ");
            string stepLogStr = Console.ReadLine();
            bool stepLog = true;
            bool actLog = true;
            if (stepLogStr == "true")
                stepLog = true;
            if (stepLogStr != "sa")
            {
                Console.WriteLine("Action Log ? (true/-)");
                Console.Write("> ");
                string actLogStr = Console.ReadLine();
                if (actLogStr == "true")
                    actLog = true;
            }
            Console.WriteLine("[Parsing (" + stepLog + actLog + ")]");

            string srcpath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "polysrc";
            Console.WriteLine("Constructing code tree...");

            PolyProgram program = new PolyProgram("TestProgram", srcpath);
            program.AddFilesFromDir(srcpath);
            program.ReadAllFiles();
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            string[] log = program.ParseAllFiles(stepLog, actLog);
            timer.Stop();
            Console.WriteLine("[Parsed: " + timer.ElapsedMilliseconds + "ms]");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (string logMsg in log)
                Console.WriteLine(logMsg);
            Console.ForegroundColor = ConsoleColor.Gray;

            if (log.Length == 0)
            {
                Console.WriteLine("[AST Tree]");
                program.Files[0].CodeTree.PrintAst();
                Console.WriteLine("[Scope Tree]");
                program.Files[0].CodeTree.PrintScope();
            }

            Console.WriteLine("[Execution]");
            PolyInterpreter interpreter = new PolyInterpreter(program.Files[0].CodeTree, new Entrypoint("Program", "Main"), new ErrorHandler());
            interpreter.Begin();

            Console.ReadLine();
        }
    }
}
