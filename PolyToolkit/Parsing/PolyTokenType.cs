using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing
{
    public enum PolyTokenType
    {
        /// <summary>
        /// Example: somename
        /// </summary>
        Name = 1,
        /// <summary>
        /// Example: 413523
        /// </summary>
        Integer = 2, // 213
        /// <summary>
        /// Example: 2131.21
        /// </summary>
        Real = 3,
        /// <summary>
        /// Example: "string"
        /// </summary>
        String = 4,
        NewLine = 5,
        /// <summary>
        /// Example: +/-/*/etc
        /// </summary>
        Operator = 6,
        /// <summary>
        /// Example: #/~/!/?/$/@/^/&
        /// </summary>
        SpecialCharacter,
        /// <summary>
        /// List: ,.;:{}()[]
        /// </summary>
        Delimiter = 7
    }
}
