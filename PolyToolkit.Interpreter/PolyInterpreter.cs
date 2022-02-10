using System;
using System.Collections;
using System.Collections.Generic;

using PolyToolkit;
using PolyToolkit.Interpreter.Library;
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
            {
                //create global scope
                Memory.NowGlobal();

                //every node in global
                foreach (AstNode coreNode in Tree.Childs)
                {
                    if (coreNode is ClassNode)
                        VisitDeclareClass((ClassNode)coreNode);
                }

                //start evaluation
                PolySymbol entryClass = Memory.GetValue(Entrypoint.Class);
                if(entryClass != null && entryClass.IsClass)
                {
                    //TODO: pass string[] args to method
                    CallClassMethod((NonComputedClass)entryClass.Value, Entrypoint.Method, new PolySymbol[] {});
                }
            }
        }

        #region Visit Statement
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
                CallExpression((ExpressionNode)node);
            #endregion
            #region Condition
            else if (node is IfNode)
                VisitIfCondition((IfNode)node, ref exitpoint);
            #endregion
            #region Loop
            else if (node is RepeatNode)
                VisitRepeatLoop((RepeatNode)node, ref exitpoint);
            #endregion
            #region Return/Break
            else if (node is ReturnStmtNode)
                exitpoint.Exit(CallExpression(((ReturnStmtNode)node).ReturnValue));
            else if (node is BreakStmtNode)
                exitpoint.Break();
            #endregion
            //unknown
            else
                ErrorHandler.ReportError(node, $"Not implemented: ({node.GetType()})");
            //TODO: declare function
            //TODO: declare class
        }
        private void VisitStatements(List<AstNode> nodes, ref Exitpoint exitpoint)
        {
            foreach (AstNode childNode in nodes)
            {
                //exit on return
                if (exitpoint.IsExited)
                    return;
                //return ?
                else if(childNode is ReturnStmtNode)
                {
                    exitpoint.Exit(CallExpression(((ReturnStmtNode)childNode).ReturnValue));
                    break;
                }
                //continue...
                else
                    VisitStatement(childNode, ref exitpoint);
            }
        }
        private void VisitDeclareVar(VarDeclarationStmtNode node)
        {
            //register declared var in memory (if value not specified -> use default value)
            Memory.Define(node.VarName, node.VarValue != null ? CallExpression(node.VarValue)
                : new PolySymbol(node.VarType, node.VarType.DefaultValue, node.IsConstant));
        }
        private void VisitDeclareField(FieldNode node)
        {
            //register field in memory (if value not specified -> use default value)
            Memory.Define(node.VarName, node.VarValue != null ? CallExpression(node.VarValue)
                : new PolySymbol(node.VarType, node.VarType.DefaultValue, node.IsConstant));
        }
        private void VisitAssignVar(VarAssignStmtNode node)
        {
            //register declared var in memory
            Memory.Current.Assign(node.VarName, CallExpression(node.VarValue));
        }

        private void VisitDeclareClass(ClassNode node)
        {
            Memory.Define(node.ClassName, new PolySymbol(PolyTypes.Class, new NonComputedClass(node), true));
        }
        private void VisitDeclareMethod(MethodNode node)
        {
            Memory.Define(node.MethodName, new PolySymbol(PolyTypes.Method, new NonComputedMethod(node), true));
        }
        #endregion

        #region Visit Condition
        private void VisitIfCondition(IfNode node, ref Exitpoint exitpoint)
        {
            //if condition met
            if((bool)CallExpression(node.Condition).Value)
                VisitStatements(node.Childs, ref exitpoint);

            //TODO: else if, else
        }
        #endregion

        #region Visit Loop
        private void VisitRepeatLoop(RepeatNode node, ref Exitpoint exitpoint)
        {
            PolySymbol times = CallExpression(node.Times);

            Exitpoint exitpointLoop = new Exitpoint();
            for (int i = 0; i < (int)times.Value; i++)
            {
                //return
                if (exitpointLoop.IsExited)
                    exitpoint.Exit(exitpointLoop.OutValue);
                //break
                else if (exitpointLoop.IsBreaked)
                    break;

                //repeat code
                VisitStatements(node.Childs, ref exitpointLoop);
            }
        }
        #endregion

        #region Call
        private PolySymbol CallClassMethod(NonComputedClass cls, string methodName, PolySymbol[] args)
        {
            PolySymbol result = new PolySymbol(PolyTypes.Unknown, null);
            Memory.NowClass(cls.Node.ClassName);

            //define methods, props in scope
            foreach (AstNode childNode in cls.Node.Childs)
            {
                if (childNode is FieldNode && !((FieldNode)childNode).IsStatic)
                    VisitDeclareField((FieldNode)childNode);
                //TODO: dont define static methods in scope
                else if (childNode is MethodNode)
                    VisitDeclareMethod((MethodNode)childNode);
            }

            PolySymbol methodSymb = Memory.GetValue(methodName);
            if (methodSymb != null && methodSymb.IsMethod)
            {
                NonComputedMethod method = (NonComputedMethod)methodSymb.Value;
                result = CallMethod(method, args);
            }
            //else
            //TODO: error

            Memory.Pop(); //remove method context

            return result;
        }
        private PolySymbol CallMethod(NonComputedMethod method, PolySymbol[] args)
        {
            Memory.NowMethod(method.Node.MethodName);

            //define arguments in scope
            if (method.Node.MethodArgs.Count > 0 && method.Node.MethodArgs.Count == args.Length)
            {
                int i = 0;
                foreach (string argName in method.Node.MethodArgs.Keys)
                {
                    Memory.Define(argName, args[i]);
                    i++;
                }
            }

            Exitpoint exitpoint = new Exitpoint(method.Node.MethodReturnType);
            //evaluate
            VisitStatements(method.Node.Childs, ref exitpoint);

            Memory.Pop(); //remove method context

            return exitpoint.IsExited ? exitpoint.OutValue : new PolySymbol(PolyTypes.Void, null);
        }

        private PolySymbol CallExpression(ExpressionNode node)
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

                //visit array items expressions, fill array value
                int i = 0;
                foreach (ExpressionNode arrayItem in ((ArrayLiteralNode)node).Values)
                {
                    val.Add(CallExpression(arrayItem));
                    i++;
                }

                return new PolySymbol(arrayType, val);
            }
            //var
            else if (node is VarNameNode)
                return Memory.GetValue(((VarNameNode)node).Name);
            //call
            else if (node is MethodCallNode)
            {
                MethodCallNode methodCallNode = (MethodCallNode)node;

                //TODO: better way to check if is library method

                //system method
                if (SystemLibrary.GetModule("@").IsHasMethod(methodCallNode.Name))
                {
                    MethodDescriptor methodDescriptor = SystemLibrary.GetModule("@").GetMethod(methodCallNode.Name);

                    //same arguments counts
                    if(methodDescriptor.Arguments.Count == methodCallNode.Args.Count)
                    {
                        //check arguments types
                        for(int i = 0; i < methodDescriptor.Arguments.Count; i++)
                        {
                            //TODO: dont check arguments types, it should be checked in parser
                            //arguments types doesn't match
                            if(methodDescriptor.Arguments[i] != PolyTypes.Object && methodDescriptor.Arguments[i] != methodCallNode.Args[i].Type)
                            {
                                ErrorHandler.ReportError(methodCallNode, $"Method '{methodCallNode.Name}' {i + 1}th argument should be of {methodDescriptor.Arguments[i]} type");
                                return new PolySymbol(PolyTypes.Null, null);
                            }
                        }

                        //evaluate arguments
                        PolySymbol[] args = new PolySymbol[methodCallNode.Args.Count];
                        for (var i = 0; i < args.Length; i++)
                        {
                            args[i] = CallExpression(methodCallNode.Args[i]);
                        }

                        //call method
                        return SystemLibraryImpl.Modules["@"].Methods[methodDescriptor.Name](args);
                    }
                    //different argument count
                    else
                        ErrorHandler.ReportError(methodCallNode, $"Method '{methodCallNode.Name}' accepts only {methodDescriptor.Arguments.Count} argument(s), but passed {methodCallNode.Args.Count}");

                    return new PolySymbol(PolyTypes.Null, null);
                }
                //defined method
                else
                {
                    PolySymbol definedSymbol = Memory.GetValue(((MethodCallNode)node).Name);

                    if (definedSymbol != null)
                    {
                        if(definedSymbol.Type == PolyTypes.Method && definedSymbol.Value is NonComputedMethod)
                        {
                            //evaluate arguments
                            PolySymbol[] args = new PolySymbol[methodCallNode.Args.Count];
                            for (var i = 0; i < args.Length; i++)
                                args[i] = CallExpression(methodCallNode.Args[i]);

                            //TODO: check if method dont accepts that arguments

                            //call method
                            return CallMethod((NonComputedMethod)definedSymbol.Value, args);
                        }
                        else
                        {
                            ErrorHandler.ReportError(node, $"Cannot call '{((MethodCallNode)node).Name}', value was {definedSymbol.Type} instead of method");
                            return new PolySymbol(PolyTypes.Unknown, null);
                        }
                    }
                    else
                    {
                        ErrorHandler.ReportError(node, $"Cannot call '{((MethodCallNode)node).Name}', method was not defined");
                        return new PolySymbol(PolyTypes.Unknown, null);
                    }
                }
            }
            //array index
            else if (node is ArrayIndexNode)
            {
                ArrayIndexNode indxnode = (ArrayIndexNode)node;

                ArrayList arr = (ArrayList)CallExpression(indxnode.Array).Value;
                int index = (int)CallExpression(indxnode.Index).Value;

                if(index < arr.Count)
                    return new PolySymbol(indxnode.Type, arr[index]);
                else
                    ErrorHandler.ReportError(node, $"Array index out of bounds (length: {arr.Count}, index: {index})");

                return new PolySymbol(PolyTypes.Unknown, null);
            }
            //operation
            else if (node is BinaryExpressionNode)
            {
                BinaryExpressionNode binNode = (BinaryExpressionNode)node;
                return CallExpression(binNode.Left).Perform(binNode.Op, CallExpression(binNode.Right));
            }
            //error
            else if(node == null)
            {
                Console.Write("null expr now");
                ErrorHandler.ReportError(node, $"Null expression");
                return new PolySymbol(PolyTypes.Unknown, null);
            }
            //not implemented
            else
            {
                ErrorHandler.ReportError(node, $"Not implemented expression ({node.GetType()})");
                return new PolySymbol(PolyTypes.Unknown, null);
            }
        }
        private PolySymbol CallNewInstance(NonComputedInstance instance, PolySymbol[] args)
        {
            return new PolySymbol(PolyTypes.Unknown, null);
        }
        #endregion
    }
}