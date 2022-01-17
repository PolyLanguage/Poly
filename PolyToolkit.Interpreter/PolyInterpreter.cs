using System;
using System.Collections;
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
        private PolyMemory Memory { get; set; }

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
            Memory = new PolyMemory();

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
                    //define Type
                    if(coreNode is ClassNode)
                    {
                        ClassNode clNode = (ClassNode)coreNode;

                        Memory.Define(clNode.ClassName, new PolySymbol(PolyTypes.ty);

                        //found entrypoint class
                        if(clNode.ClassName == this.Entrypoint.Class)
                        {
                            // Create class scope
                            Memory.AddScope(new PolyScope(this.Entrypoint.Class, PolyScope.Container.Class));

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
                                    Memory.DefineInCurrent(methodNode.MethodName, new PolySymbol(PolyTypes.Function, new NonComputed(methodNode)));

                                    //found entrypoint method
                                    if (((MethodNode)memberNode).MethodName == this.Entrypoint.Method)
                                    {
                                        // Create method scope
                                        Memory.AddScope(new PolyScope(this.Entrypoint.Method, PolyScope.Container.Method));

                                        Visit(memberNode);
                                    }
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
            return new PolySymbol(PolyTypes.Null, null);
        }
        private PolySymbol VisitMethod(MethodNode node, PolySymbol[] args)
        {
            PolyScope lastMemory = Memory.Pop();
            Memory.Push(new PolyScope(node.MethodName, PolyScope.Container.Method));

            //define arguments in scope
            if (node.MethodArgs.Count > 0 && node.MethodArgs.Count == args.Length)
            {
                int i = 0;
                foreach(string argName in node.MethodArgs.Keys)
                {
                    Memory.Peek().Define(argName, args[i]);
                    i++;
                }
            }

            Exitpoint exitpoint = new Exitpoint(node.MethodReturnType);

            //evaluate
            foreach (AstNode childNode in node.Childs)
            {
                //exit on return
                if (exitpoint.IsExited)
                    break;
                //continue...
                else
                    VisitStatement(childNode, ref exitpoint);
            }

            Memory.Pop(); //remove method context
            Memory.Push(lastMemory); //back to latest context

            return exitpoint.IsExited ? exitpoint.OutValue : new PolySymbol(PolyTypes.Void, null);
        }
        #endregion

        #region Statement
        private void VisitStatement(AstNode node, ref Exitpoint exitpoint)
        {
            #region Statement
            //declare var
            if (node is VarDeclarationStmtNode)
                VisitDeclareVar((VarDeclarationStmtNode)node);
            //assign var
            else if (node is VarAssignStmtNode)
                VisitAssignVar((VarAssignStmtNode)node);
            //expression
            else if (node is ExpressionNode)
                VisitExpression((ExpressionNode)node);
            #endregion
            #region Condition
            else if (node is IfNode)
                VisitIfCondition((IfNode)node, ref exitpoint);
            #endregion
            #region Return
            else if(node is ReturnStmtNode)
                exitpoint.Exit(VisitExpression(((ReturnStmtNode)node).ReturnValue).Value);
            #endregion
            //unknown
            else
                ErrorHandler.ReportError(node, "Not implemented");
            //TODO: declare function
            //TODO: declare class
        }
        #endregion

        #region Visit Statement
        private void VisitDeclareVar(VarDeclarationStmtNode node)
        {
            //register declared var in memory
            Memory.Peek().Define(node.VarName, node.VarValue != null ? VisitExpression(node.VarValue)
                : new PolySymbol(node.VarType, node.VarType.DefaultValue));
        }
        private void VisitAssignVar(VarAssignStmtNode node)
        {
            //register declared var in memory
            Memory.Peek().Assign(node.VarName, VisitExpression(node.VarValue));
        }
        #endregion

        #region Visit Condition
        private void VisitIfCondition(IfNode node, ref Exitpoint exitpoint)
        {
            if((bool)VisitExpression(node.Condition).Value)
            {
                foreach (AstNode childNode in node.Childs)
                    VisitStatement(childNode, ref exitpoint);
            }
            //TODO: else if, else
        }
        #endregion

        #region Expression
        private PolySymbol VisitExpression(ExpressionNode node)
        {
            //literals
            if (node is NullLiteralNode)
                return new PolySymbol(PolyTypes.Null, null);
            else if (node is IntLiteralNode)
                return new PolySymbol(PolyTypes.Int, ((IntLiteralNode)node).Value);
            else if (node is RealLiteralNode)
                return new PolySymbol(PolyTypes.Real, ((RealLiteralNode)node).Value);
            else if (node is StringLiteralNode)
                return new PolySymbol(PolyTypes.String, ((StringLiteralNode)node).Value);
            else if (node is BoolLiteralNode)
                return new PolySymbol(PolyTypes.Bool, ((BoolLiteralNode)node).Value);
            else if (node is ArrayLiteralNode)
            {
                PolyType arrayType = ((ArrayLiteralNode)node).Type;

                //create array
                ArrayList val = new ArrayList();
                //Array val = Array.CreateInstance(arrayType.ToNativeType(), 1000); //TODO: do something with length

                //visit array items expressions, fill array value
                int i = 0;
                foreach (ExpressionNode arrayItem in ((ArrayLiteralNode)node).Values)
                {
                    val.Add(VisitExpression(arrayItem).Value);
                    //val.SetValue(VisitExpression(arrayItem).Value, i);
                    i++;
                }

                return new PolySymbol(arrayType, val);
            }
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

                    return new PolySymbol(PolyTypes.Null, null);
                }
                //defined method
                else
                {
                    PolySymbol method = GetValue(((MethodCallNode)node).Name);

                    if (method.Type == PolyTypes.Function && method.Value is NonComputed)
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
                        return new PolySymbol(PolyTypes.Unknown, null);
                    }
                }
            }
            //array index
            else if (node is ArrayIndexNode)
            {
                ArrayIndexNode indxnode = (ArrayIndexNode)node;

                ArrayList arr = (ArrayList)GetValue(indxnode.Name).Value;
                //Array arr = (Array)GetValue(indxnode.Name).Value;
                int index = (int)VisitExpression(indxnode.Index).Value;

                return new PolySymbol(indxnode.Type, arr[index]);
                //return new PolySymbol(indxnode.Type, arr.GetValue(index));
            }
            //operation
            else if (node is BinaryExpressionNode)
            {
                BinaryExpressionNode binNode = (BinaryExpressionNode)node;
                return VisitExpression(binNode.Left).Perform(binNode.Op, VisitExpression(binNode.Right));
            }
            //unknown
            else
            {
                ErrorHandler.ReportError(node, "Not implemented expression");
                return new PolySymbol(PolyTypes.Unknown, null);
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
        private bool IsDefined(string name)
        {
            // loop all scopes
            foreach (PolyScope scope in Memory)
            {
                // defined in current scope
                if (scope.IsDefined(name))
                    return true;
            }

            // not found
            return false;
        }
        #endregion
    }
}