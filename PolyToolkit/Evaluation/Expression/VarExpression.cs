using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    /// <summary>
    /// Name of variable in expression. Example: myvar1
    /// </summary>
    public class VarExpression : IExpression
    {
        public string VarName { get; }
        /// <summary>
        /// Any type(object)
        /// </summary>
        public PolyType Type { get { return PolyType.ObjectType; } }

        public VarExpression(string name)
        {
            VarName = name;
        }
        public object Evaluate(ICurrentContext context)
        {
            return context.GetValue(VarName);
        }
    }
}
