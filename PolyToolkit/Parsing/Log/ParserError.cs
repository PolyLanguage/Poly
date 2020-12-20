using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing
{
    public class ParserError
    {
        public string Message { get; }
        public int Line { get; }
        public ThrowedIn ThrowedIn { get; }

        public override string ToString()
        {
            return Message;
        }

        public ParserError(string msg,int line,ThrowedIn thrower)
        {
            Message = GenerateExcText(msg, line);
            Line = line;
            ThrowedIn = thrower;
        }
        public ParserError(string msg, int line,string after,ThrowedIn thrower)
        {
            Message = GenerateExcText(msg, line, after);
            Line = line;
            ThrowedIn = thrower;
        }

        private static string GenerateExcText(string msg,int line)
        {
            if (line != -1)
                return msg + " at " + "line " + line.ToString() + ".";
            else
                return msg+".";
        }
        private static string GenerateExcText(string msg,int line,string after)
        {
            return GenerateExcText(msg, line) + after;
        }
    }
}
