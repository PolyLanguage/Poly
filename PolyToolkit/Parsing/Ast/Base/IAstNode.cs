using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Parsing.Ast
{
    public interface IAstNode
    {
        IAstNode Parent { get; set; }
        List<IAstNode> Childs { get; }
    }
}
