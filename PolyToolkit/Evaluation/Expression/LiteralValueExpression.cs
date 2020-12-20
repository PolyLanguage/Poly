using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit.Evaluation
{
    /// <summary>
    /// Literal Value. Example: null/true/false
    /// </summary>
    public class LiteralValueExpression : IExpression
    {
        public object LiteralValue { get; }
        public PolyType Type { get; }

        public LiteralValueExpression(string literal)
        {
            LiteralValue = PolyType.LiteralToNative(literal);
            Type = PolyType.IdentifyLiteralType(LiteralValue);
        }

        public object Evaluate(ICurrentContext context)
        {
            return LiteralValue;
        }
    }
}
