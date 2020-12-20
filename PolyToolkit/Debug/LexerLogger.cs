using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Parsing;
namespace PolyToolkit.Debug
{
    public class LexerLogger
    {
        public List<LexerError> Errors { get; }

        public LexerLogger()
        {
            Errors = new List<LexerError>();
        }

        public void Add(LexerError err)
        {
            Errors.Add(err);
        }
    }
}
