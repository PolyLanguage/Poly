using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Evaluation;
namespace PolyToolkit.Evaluation
{
    /// <summary>
    /// Value. Example: 100/1.0/"hello"
    /// </summary>
    public class ConstExpression : IExpression
    {
        public object ConstValue { get; }
        public PolyType Type { get; }

        public ConstExpression(object value)
        {
            ConstValue = value;
            Type = PolyType.IdentifyValue(value);
        }

        public object Evaluate(ICurrentContext context)
        {
            return ConstValue;
        }
    }
}
