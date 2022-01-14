using System;
using System.Collections.Generic;

using PolyToolkit;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Interpreter
{
    /// <summary>
    /// Interprets poly code
    /// </summary>
    public sealed class PolyInterpreter
    {
        private Stack<PolyScope> Memory { get; set; }

        private CodeTree Tree { get; set; }
        private Entrypoint Entrypoint { get; set; }
        private ErrorHandler ErrorHandler { get; set; }

        /// <summary>
        /// Initialize interpreter
        /// </summary>
        /// <param name="tree">Code AST</param>
        /// <param name="codeEntrypoint">Start of the code</param>
        public PolyInterpreter(CodeTree tree, Entrypoint codeEntrypoint, ErrorHandler errHandler)
        {
            Memory = new Stack<PolyScope>();

            Tree = tree;
            Entrypoint = codeEntrypoint;
            ErrorHandler = errHandler;
        }

        /// <summary>
        /// Begin interpretation of the code
        /// </summary>
        public void Begin()
        {
            if(Tree == null || Tree.Childs == null)
            {
                ErrorHandler.ReportError(Tree, "Code was empty");
            }
            else
                foreach(AstNode coreNode in Tree.Childs)
                {
                    //found entrypoint class
                    if (coreNode is ClassNode && ((ClassNode)coreNode).ClassName == this.Entrypoint.Class)
                    {
                        // Create class scope
                        Memory.Push(new PolyScope(this.Entrypoint.Class, PolyScope.Container.Class));

                        foreach (AstNode memberNode in coreNode.Childs)
                        {
                            // field
                            if (memberNode is VarDeclarationStmtNode)
                                VisitDeclareVar((VarDeclarationStmtNode)memberNode);
                            // method
                            else if (memberNode is MethodNode)
                            {
                                MethodNode methodNode = (MethodNode)memberNode;

                                // Define method in class scope
                                Memory.Peek().Define(methodNode.MethodName, new PolySymbol(PolyType.FunctionType, new NonComputed(methodNode)));

                                //found entrypoint method
                                if (((MethodNode)memberNode).MethodName == this.Entrypoint.Method)
                                {
                                    // Create method scope
                                    Memory.Push(new PolyScope(this.Entrypoint.Method, PolyScope.Container.Method));

                                    Visit(memberNode);
                                }
                            }
                        }
                    }
                }    
        }

        private void Visit(AstNode node, PolySymbol[] args = null)
        {
            //method
            if (node is MethodNode)
                VisitMethod((MethodNode)node, args);
            //ctor
            else if (node is ClassCtorNode)
                VisitCtor((ClassCtorNode)node, args);
        }

        #region Core
        private PolySymbol VisitCtor(ClassCtorNode node, PolySymbol[] args)
        {
            //TODO: return class instance
            return new PolySymbol(PolyType.NullType, null);
        }
        private PolySymbol VisitMethod(MethodNode node, PolySymbol[] args)
        {
            Memory.Push(new PolyScope(node.MethodName, PolyScope.Container.Method));

            //define arguments in scope
            if(node.MethodArgs.Count > 0 && node.MethodArgs.Count == args.Length)
            {
                int i = 0;
                foreach(string argName in node.MethodArgs.Keys)
                {
                    Memory.Peek().Define(argName, args[i]);
                    i++;
                }
            }

            //evaluate
            foreach (AstNode childNode in node.Childs)
            {
                //return statement
                if (node.MethodReturnType != PolyType.NoneType && childNode is ReturnStmtNode)
                    return VisitExpression(((ReturnStmtNode)childNode).ReturnValue);
                else
                    VisitStatement(childNode);
            }

            //method has no return type
            return new PolySymbol(PolyType.NoneType, null);
        }
        #endregion

        #region Statement
        private void VisitStatement(AstNode node)
        {
            if (node is VarDeclarationStmtNode)
                VisitDeclareVar((VarDeclarationStmtNode)node);
            else if (node is VarAssignStmtNode)
                ; //TODO: assign variable
            else if (node is ExpressionNode)
                VisitExpression((ExpressionNode)node);
            else
                ErrorHandler.ReportError(node, "Not implemented");
            //TODO: evaluate expression
            //TODO: declare function
            //TODO: declare class
        }
        private void VisitDeclareVar(VarDeclarationStmtNode node)
        {
            //register declared var in memory
            Memory.Peek().Define(node.VarName, VisitExpression(node.VarValue));
        }
        #endregion

        #region Expression
        private PolySymbol VisitExpression(ExpressionNode node)
        {
            //literals
            if (node is NullLiteralNode)
                return new PolySymbol(PolyType.NullType, null);
            else if (node is IntLiteralNode)
                return new PolySymbol(PolyType.IntType, ((IntLiteralNode)node).Value);
            else if (node is RealLiteralNode)
                return new PolySymbol(PolyType.RealType, ((RealLiteralNode)node).Value);
            else if (node is StringLiteralNode)
                return new PolySymbol(PolyType.StringType, ((StringLiteralNode)node).Value);
            else if (node is BoolLiteralNode)
                return new PolySymbol(PolyType.BooleanType, ((BoolLiteralNode)node).Value);
            //var
            else if (node is VarNameNode)
                return GetValue(((VarNameNode)node).Name);
            //call
            else if (node is MethodCallNode)
            {
                MethodCallNode methodCallNode = (MethodCallNode)node;

                //TODO: better way to check if is library method

                //system method
                if (methodCallNode.Name == "print")
                {
                    //evaluate arguments
                    PolySymbol[] args = new PolySymbol[methodCallNode.Args.Count];
                    for (var i = 0; i < args.Length; i++)
                    {
                        args[i] = VisitExpression(methodCallNode.Args[i]);
                    }

                    if (args.Length == 1)
                        Console.WriteLine(args[0].Value);
                    else
                        ErrorHandler.ReportError(methodCallNode, "Method accepts only 1 argument");

                    return new PolySymbol(PolyType.NullType, null);
                }
                //defined method
                else
                {
                    PolySymbol method = GetValue(((MethodCallNode)node).Name);

                    if (method.Type == PolyType.FunctionType && method.Value is NonComputed)
                    {
                        //evaluate arguments
                        PolySymbol[] args = new PolySymbol[methodCallNode.Args.Count];
                        for (var i = 0; i < args.Length; i++)
                            args[i] = VisitExpression(methodCallNode.Args[i]);

                        //TODO: check if method dont accepts that arguments

                        return VisitMethod((MethodNode)((NonComputed)method.Value).Node, args);
                    }
                    else
                    {
                        ErrorHandler.ReportError(node, $"Cannot call '{((VarNameNode)node).Name}', value was {method.Type} instead of method");
                        return new PolySymbol(PolyType.UnknownType, null);
                    }
                }
            }
            //arithmetic
            else if (node is AddExpressionNode)
            {
                AddExpressionNode addnode = (AddExpressionNode)node;

                return VisitExpression(addnode.Left) + VisitExpression(addnode.Right);
            }
            else if (node is SubtractExpressionNode)
            {
                SubtractExpressionNode subnode = (SubtractExpressionNode)node;
                return VisitExpression(subnode.Left) - VisitExpression(subnode.Right);
            }
            //unknown
            else
            {
                ErrorHandler.ReportError(node, "Not implemented expression");
                return new PolySymbol(PolyType.UnknownType, null);
            }
        }
        private PolySymbol GetValue(string name)
        {
            // loop all scopes
            foreach(PolyScope scope in Memory)
            {
                // defined in current scope
                if (scope.IsDefined(name))
                    return scope.Get(name);
            }

            // not found
            return null;
        }
        #endregion
    }
}
