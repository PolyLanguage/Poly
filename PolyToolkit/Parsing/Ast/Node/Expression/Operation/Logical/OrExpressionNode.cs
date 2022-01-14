using System;

namespace PolyToolkit.Parsing.Ast
{
    public sealed class OrExpressionNode : BooleanExpressionNode
    {
        private Func<object, object, bool> apply;

        public OrExpressionNode(AstNode parent, int line)
            : base(parent, line)
        { }

        public override void ApplyType()
        {
            base.ApplyType();

            if (Left != null && Right != null)
            {
                if (Left.Type == PolyType.BooleanType && Right.Type == PolyType.BooleanType)
                    this.apply = (a, b) => (bool)a || (bool)b;
            }
        }
        /*public override object Apply(object leftvalue, object rightvalue)
        {
            if (this.apply != null)
                return this.apply(leftvalue, rightvalue);

            return null;
        }*/
    }
}
