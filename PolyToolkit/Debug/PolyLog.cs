using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Debug
{
    public class PolyLog
    {
        public ParserLogger ParserLog { get; }
        public LexerLogger LexerLog { get; }

        public PolyLog(ParserLogger p,LexerLogger l)
        {
            ParserLog = p;
            LexerLog = l;
        }
    }
}
