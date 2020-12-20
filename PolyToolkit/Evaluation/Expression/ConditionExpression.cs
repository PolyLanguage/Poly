using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public class ConditionExpression : IExpression
    {
        private IExpression conditionExpr;
        private IExpression thenExpr;
        private IExpression elseExpr;
        public PolyType Type { get; }

        public ConditionExpression(IExpression conditionexpression, IExpression thenexpression,
            IExpression elseexpression)
        {
            conditionExpr = conditionexpression;
            thenExpr = thenexpression;
            elseExpr = elseexpression;
            Type = thenexpression.Type;
        }

        public object Evaluate(ICurrentContext context)
        {
            bool condition = (bool)this.conditionExpr.Evaluate(context);

            if (condition)
                return this.thenExpr.Evaluate(context);
            else
                return this.elseExpr.Evaluate(context);
        }
    }
}
