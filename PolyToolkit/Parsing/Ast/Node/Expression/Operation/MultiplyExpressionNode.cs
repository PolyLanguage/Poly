﻿using System;

namespace PolyToolkit.Parsing.Ast
{
    public class MultiplyExpressionNode : ArithmeticExpressionNode
    {
        public override IExpressionNode Left { get; protected set; }
        public override IExpressionNode Right { get; protected set; }

        public MultiplyExpressionNode(IAstNode parent, IExpressionNode left, IExpressionNode right)
            : base(parent, left, right)
        { }

        /*public override object Apply(object leftvalue, object rightvalue)
        {
            if (leftvalue is int)
                if (rightvalue is int)
                    return (int)leftvalue * (int)rightvalue;
                else
                    return (int)leftvalue * (double)rightvalue;
            else if (rightvalue is int)
                return (double)leftvalue * (int)rightvalue;
            else if(leftvalue is double && rightvalue is double)
                return (double)leftvalue * (double)rightvalue;
            else if(leftvalue is string && rightvalue is int)
                return 
        }*/
    }
}