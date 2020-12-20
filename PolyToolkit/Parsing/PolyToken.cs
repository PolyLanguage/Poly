using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing
{
    public class PolyToken
    {
        public string Value { get; }
        public PolyTokenType Type { get; }

        public PolyToken(string val,PolyTokenType type)
        {
            Value = val;
            Type = type;
        }
    }
}
