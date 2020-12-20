using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Parsing.Ast
{
    public interface IExpressionNode : IAstNode
    {
        /// <summary>
        /// Type of the expression
        /// </summary>
        PolyType Type { get; }
    }
}
