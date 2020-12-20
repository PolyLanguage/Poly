using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public interface IExpression
    {
        PolyType Type { get; }
        object Evaluate(ICurrentContext context);
    }
}
