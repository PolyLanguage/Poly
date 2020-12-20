using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Parsing;
namespace PolyToolkit.Debug
{
    public class ParserLogger
    {
        public List<ParserError> Errors { get; private set; }
        public List<ParserWarning> Warnings { get; private set; }

        public ParserLogger()
        {
            Errors = new List<ParserError>();
            Warnings = new List<ParserWarning>();
        }

        public void Add(ParserError err)
        {
            Errors.Add(err);
        }
        public void Add(ParserWarning warn)
        {
            Warnings.Add(warn);
        }
    }
}
