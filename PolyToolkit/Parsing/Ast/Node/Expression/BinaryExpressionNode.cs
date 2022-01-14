using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class BinaryExpressionNode : ExpressionNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>() { Left,Right }; set => throw new InvalidOperationException("Childs of this node cannot be set"); }

        /// <summary>
        /// Left part of the expression
        /// </summary>
        public ExpressionNode Left { get { return _left; } set { _left = value; ApplyType(); } }
        private ExpressionNode _left;
        /// <summary>
        /// Right part of the expression
        /// </summary>
        public ExpressionNode Right { get { return _right; } set { _right = value; ApplyType(); } }
        private ExpressionNode _right;

        public BinaryExpressionNode(AstNode parent, int line) : base(parent, line) { }

        public virtual void ApplyType()
        {
            if (Left != null && Right != null)
            {
                //left & right expressions is int -> this expression is int
                if (Left.Type == PolyType.IntType && Right.Type == PolyType.IntType)
                    Type = PolyType.IntType;
                //left | right expression is boolean -> this expression is boolean
                else if (Left.Type == PolyType.BooleanType || Right.Type == PolyType.BooleanType)
                    Type = PolyType.BooleanType;
                else if (Left.Type == PolyType.RealType || Right.Type == PolyType.RealType)
                    Type = PolyType.RealType;
                else if (Left.Type == PolyType.ObjectType || Right.Type == PolyType.ObjectType)
                    Type = PolyType.ObjectType;
                else
                    Type = PolyType.UnknownType;
            }
        }

        public bool IsTypesValid()
        {
            if (Left != null && Right != null)
            {
                return Left.Type == Right.Type;
            }
            else
                return true; // Ignore unknown types
        }
    }
}
