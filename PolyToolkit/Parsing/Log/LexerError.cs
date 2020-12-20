using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing
{
    public class LexerError
    {
        public string Message { get; }
        public override string ToString()
        {
            return Message;
        }

        public LexerError(string msg)
        {
            Message = msg;
        }
    }
}
