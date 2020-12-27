using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class BinaryExpressionNode : IExpressionNode
    {
        public IAstNode Parent { get; set; }
        public List<IAstNode> Childs { get { return new List<IAstNode>() { Left, Right }; } }

        /// <summary>
        /// Left part of the expression
        /// </summary>
        public IExpressionNode Left { get { return _left; } set { _left = value; ApplyType(); } }
        private IExpressionNode _left;
        /// <summary>
        /// Right part of the expression
        /// </summary>
        public IExpressionNode Right { get { return _right; } set { _right = value; ApplyType(); } }
        private IExpressionNode _right;

        public PolyType Type { get; protected set; }

        public BinaryExpressionNode(IAstNode parent)
        {
            Parent = parent;
        }

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
            if(Left != null && Right != null)
            {
                return Left.Type == Right.Type;
            }

            return false;
        }
    }
}
