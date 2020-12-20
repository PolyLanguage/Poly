using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing
{
    public class ParserWarning
    {
        public string Message { get; }
        public int Line { get; }

        public override string ToString()
        {
            return Message;
        }
        public ParserWarning()
        {

        }
    }
}
