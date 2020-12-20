using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Debug
{
    /// <summary>
    /// Used to debug code in runtime(breakpoint/runtime exception)
    /// </summary>
    public class CodeDebugger
    {
        //TODO: event on runtime error
        public int CodeBreakpoint { get; private set; }

        public void SetBreakpoint(int line)
        {
            CodeBreakpoint = line;
        }
    }
}
