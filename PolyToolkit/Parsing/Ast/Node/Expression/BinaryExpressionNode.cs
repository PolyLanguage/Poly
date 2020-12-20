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
        public abstract IExpressionNode Left { get; protected set; }
        /// <summary>
        /// Right part of the expression
        /// </summary>
        public abstract IExpressionNode Right { get; protected set; }

        public PolyType Type { get; protected set; }

        public BinaryExpressionNode(IAstNode parent, IExpressionNode left,IExpressionNode right)
        {
            Parent = parent;

            Left = left;
            Right = right;

            //left & right expressions is int -> this expression is int
            if (left.Type == PolyType.IntType && right.Type == PolyType.IntType)
                Type = PolyType.IntType;
            //left | right expression is boolean -> this expression is boolean
            else if (left.Type == PolyType.BooleanType || right.Type == PolyType.BooleanType)
                Type = PolyType.BooleanType;
            else //else --> this expression is real
                Type = PolyType.RealType;
        }
    }
}
