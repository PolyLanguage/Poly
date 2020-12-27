using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit;
namespace PolyToolkit.Parsing.Ast
{
    public interface IWithBody : IAstNode
    {
        bool IsAllowed<T>() where T : IAstNode;
    }
}
