using System;

namespace PolyToolkit.Parsing.Ast
{
    public sealed class MoreThanExpressionNode : BooleanExpressionNode
    {
        private Func<object, object, bool> apply;

        public MoreThanExpressionNode(IAstNode parent)
            : base(parent)
        { }

        public override void ApplyType()
        {
            base.ApplyType();

            if (Left != null && Right != null)
            {
                if (Left.Type == PolyType.IntType && Right.Type == PolyType.IntType)
                    this.apply = (a, b) => (int)a > (int)b;
                else if (Left.Type == PolyType.RealType && Right.Type == PolyType.RealType)
                    this.apply = (a, b) => (double)a > (double)b;
                else if (Left.Type == PolyType.IntType && Right.Type == PolyType.RealType)
                    this.apply = (a, b) => (int)a > (double)b;
                else if (Left.Type == PolyType.RealType && Right.Type == PolyType.IntType)
                    this.apply = (a, b) => (double)a > (int)b;
            }
        }
        /*public override object Apply(object leftvalue, object rightvalue)
        {
            if (this.apply != null)
                return this.apply(leftvalue, rightvalue);

            return null;
        }*/
    }

    public sealed class MoreThanOrEqualsExpressionNode : BooleanExpressionNode
    {
        private Func<object, object, bool> apply;

        public MoreThanOrEqualsExpressionNode(IAstNode parent)
            : base(parent)
        { }

        public override void ApplyType()
        {
            base.ApplyType();

            if (Left != null && Right != null)
            {
                if (Left.Type == PolyType.IntType && Right.Type == PolyType.IntType)
                    this.apply = (a, b) => (int)a >= (int)b;
                else if (Left.Type == PolyType.RealType && Right.Type == PolyType.RealType)
                    this.apply = (a, b) => (double)a >= (double)b;
                else if (Left.Type == PolyType.IntType && Right.Type == PolyType.RealType)
                    this.apply = (a, b) => (int)a >= (double)b;
                else if (Left.Type == PolyType.RealType && Right.Type == PolyType.IntType)
                    this.apply = (a, b) => (double)a >= (int)b;
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
