using System;
using System.Collections.Generic;

namespace PolyToolkit.Parsing.Ast
{
    public abstract class BinaryExpressionNode : ExpressionNode
    {
        public override List<AstNode> Childs { get => new List<AstNode>() { Left, Right }; set => throw new InvalidOperationException("Childs of this node cannot be set"); }
        public abstract MathOperation Op { get; }

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

        protected virtual void ApplyType()
        {}

        public bool IsTypesValid()
        {
            return Left.Type.IsPerformable(Op, Right.Type);
        }
    }
}
