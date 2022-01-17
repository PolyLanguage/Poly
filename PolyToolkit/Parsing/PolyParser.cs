using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using PolyToolkit.Parsing.Ast;
using PolyToolkit.Debug;
namespace PolyToolkit.Parsing
{
    public class PolyParser
    {
        //private
        private static string[][] BinaryOps = new string[][]
        {
            new string[] { "&&", "||" }, //level 0
            new string[] { "==", "!=", "<",">","<=",">=" }, //level 1
            new string[] { "+", "-" }, //level 2
            new string[] { "*", "/", "%" } //level 3
        };

        private PolyLexer Lexer;
        private Stack<PolyToken> Toks = new Stack<PolyToken>();
        private Stack<Dictionary<string, PolyType>> Scope = new Stack<Dictionary<string, PolyType>>();

        //public
        public int CurrentLine = 1;

        //logs
        public bool DoStepLog { get; set; } = false;
        public bool DoActionLog { get; set; } = false;

        public ParserLogger Log { get; }
        public LexerLogger LexLog { get { return Lexer.Log; } }

        //context
        public CodeTree CurrentTree { get; set; }

        public PolyParser(string code)
        {
            Lexer = new PolyLexer(code);
            Log = new ParserLogger();
        }

        #region Parse Core
        /// <summary>
        /// Parse code
        /// </summary>
        /// <returns></returns>
        public CodeTree ParseCode()
        {
            CurrentTree = new CodeTree(new List<AstNode>());

            for (var node = this.ParseNode(CurrentTree, null);
                node != null;
                node = this.ParseNode(CurrentTree, null))
            {
                //its import stmt or class
                if (node is UnknownNode == false)
                {
                    if(node is NamespaceStmtNode == false)
                        CurrentTree.Childs.Add(node);
                    else
                    {
                        if (CurrentTree.IsContainsNodeIn<NamespaceStmtNode>() == false)
                            CurrentTree.Childs.Add(node);
                        else
                            Log.Add(new ParserError("Unexpected namespace statement", CurrentLine,
                                "You cannot define namespace second time.", ThrowedIn.CodeTreeParse));
                    }
                }
            }

            //if not contains namespace definition
            if (!CurrentTree.IsContainsNodeIn<NamespaceStmtNode>())
                Log.Add(new ParserError("Undefined namespace", 1, ThrowedIn.CodeTreeParse));

            if (Log.Errors.Count == 0 && Lexer.Log.Errors.Count == 0)
                return CurrentTree; //return tree & attach context
            else
                return null;
        }

        /// <summary>
        /// Parse current node
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        private AstNode ParseNode(BlockNode parent,string until)
        {
            PolyToken token = this.NextToken();

            if (token == null)
                return null;

            #region Core
            //import
            if (token.Type == PolyTokenType.Name && token.Value == "import")
            {
                if (parent.IsAllowed<ImportStmtNode>())
                {
                    StepSuccess();
                    return ParseImportStmt(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected import statement", CurrentLine, ThrowedIn.NodeParse));
            }
            //namespace
            else if (token.Type == PolyTokenType.Name && token.Value == "namespace")
            {
                if (parent.IsAllowed<NamespaceStmtNode>())
                {
                    StepSuccess();
                    return ParseNamespaceStmt(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected namespace statement", CurrentLine, ThrowedIn.NodeParse));
            }
            #endregion
            #region OOP
            //class
            else if (token.Type == PolyTokenType.Name && token.Value == "class")
            {
                if (parent.IsAllowed<ClassNode>())
                {
                    StepSuccess();
                    return ParseClass(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected class declaration", CurrentLine, ThrowedIn.NodeParse));
            }
            //method
            else if(token.Type == PolyTokenType.Name && token.Value == "method")
            {
                if(parent.IsAllowed<MethodNode>())
                {
                    StepSuccess();
                    return ParseMethod(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected method", CurrentLine, ThrowedIn.NodeParse));
            }
            //constructor
            else if(token.Type == PolyTokenType.Name && token.Value == "ctor")
            {
                if(parent.IsAllowed<ClassCtorNode>())
                {
                    StepSuccess();
                    return ParseClassCtor(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected ctor", CurrentLine, ThrowedIn.NodeParse));
            }
            #endregion
            #region Conditions
            //if condition
            else if (token.Type == PolyTokenType.Name && token.Value == "if")
            {
                if (parent.IsAllowed<IfNode>())
                {
                    StepSuccess();
                    return ParseIf(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected 'if' condition", CurrentLine, ThrowedIn.NodeParse));
            }
            //TODO: else if, else
            #endregion
            #region Statement
            //var declaration (<TYPE> <NAME> = <EXPRESSION>;)
            else if (token.Type == PolyTokenType.Name && PolyTypes.IsItTypeName(token.Value))
            {
                if (parent.IsAllowed<VarDeclarationStmtNode>())
                {
                    PushToken(token);
                    return ParseVarDeclaration(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected declaration statement", CurrentLine, ThrowedIn.NodeParse));
            }
            //var assign (<NAME> = <EXPRESSION>;)
            else if (token.Type == PolyTokenType.Name &&
                parent.IsNameIn(token.Value) && NextTokenIs(PolyTokenType.Operator, "=", true))
            {
                if (parent.IsAllowed<VarAssignStmtNode>())
                {
                    PushToken(token);
                    return ParseVarAssignation(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected assign statement", CurrentLine, ThrowedIn.NodeParse));
            }
            //return (return <EXPRESSION>;)
            else if(token.Type == PolyTokenType.Name && token.Value == "return")
            {
                if(parent.IsAllowed<ReturnStmtNode>())
                {
                    StepSuccess();
                    return ParseReturnStmt(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected return statement", CurrentLine, ThrowedIn.NodeParse));
            }
            #endregion
            //end
            else if (token.Value == until)
            {
                //push back
                PushToken(token);

                //return end
                return null;
            }
            //expression/unknown
            else
            {
                PushToken(token);

                ExpressionNode expr = ParseBinaryExpression(0, parent);

                ParseStatementEnd(); //;

                if (expr != null)
                {
                    StepSuccess();
                    return expr;
                }
                else
                {
                    Log.Add(new ParserError("Unknown or unexpected statement('" + token.Value + "')", CurrentLine, ThrowedIn.NodeParse));
                    return null;
                }
            }

            return new UnknownNode(parent, CurrentLine);
        }
        #endregion

        #region Parse Statement
        //import <STATIC_STR>;
        private ImportStmtNode ParseImportStmt(AstNode parent)
        {
            StepInfo("Parsing Import");
            
            //example: import: <STATIC_STR>;
            ImportStmtNode node = new ImportStmtNode(parent, CurrentLine);

            node.ImportValue = ParseStringLiteral(node);
            ParseStatementEnd();

            return node;
        }
        //namespace <STATIC_STR>;
        private NamespaceStmtNode ParseNamespaceStmt(AstNode parent)
        {
            StepInfo("Parsing Namespace");

            //example: namespace <STATIC_STR>;
            NamespaceStmtNode node = new NamespaceStmtNode(parent, CurrentLine);

            node.NamespaceValue = ParseStringLiteral(node);
            ParseStatementEnd();

            return node;
        }

        //return <EXPR>;
        private ReturnStmtNode ParseReturnStmt(AstNode parent)
        {
            StepInfo("Parsing Return");

            //example: return <EXPR>;
            ReturnStmtNode node = new ReturnStmtNode(parent, CurrentLine);

            node.ReturnValue = ParseBinaryExpression(0, node);
            ParseStatementEnd();

            //if method returns none
            MethodNode meth = node.GetFirstParent<MethodNode>();
            if (meth.MethodReturnType == PolyTypes.Void)
                Log.Add(new ParserError($"Method '{meth.MethodName}' cannot return anything",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            else if(meth.MethodReturnType != node.ReturnValue.Type)
                Log.Add(new ParserError($"Method '{meth.MethodName}' should return {meth.MethodReturnType.Name} value",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return node;
        }
        #endregion

        #region Parse Class
        //class <NAME> { <ALLOWED-NODES> }
        private ClassNode ParseClass(AstNode parent)
        {
            StepInfo("Parsing Class");

            ClassNode clnode = new ClassNode(parent, CurrentLine);

            //parse class name
            clnode.ClassName = ParseName();

            //expect block start
            ParseCodeblockStart();

            //parse nodes until '}' (var/method)
            for (var node = this.ParseNode(clnode,"}");
                node != null;
                node = this.ParseNode(clnode,"}"))
            {
                //not unknown
                if(node is UnknownNode == false)
                    clnode.Childs.Add(node);
            }

            //expect block end
            ParseCodeblockEnd();

            return clnode;
        }
        //ctor (<ARGS>) { <ALLOWED-NODES> }
        private ClassCtorNode ParseClassCtor(AstNode parent)
        {
            StepInfo("Parsing Class Constructor");

            //inst
            ClassCtorNode ctornode = new ClassCtorNode(parent, CurrentLine);

            //identification
            ctornode.CtorArgs = ParseDefArgs(ctornode);

            ParseCodeblockStart(); // {

            //parse nodes until '}'
            for (var node = this.ParseNode(ctornode, "}");
                node != null;
                node = this.ParseNode(ctornode, "}"))
            {
                //declare var OR etc
                if (node is UnknownNode == false)
                    ctornode.Childs.Add(node);
            }

            ParseCodeblockEnd(); // }

            return ctornode;
        }
        #endregion

        #region Parse Variable
        //<TYPE> <NAME> = <EXPRESSION>; //TODO: DICT DECLARATION, STACK DECLARATION
        private VarDeclarationStmtNode ParseVarDeclaration(AstNode parent)
        {
            StepInfo("Parsing Var Declaration");

            VarDeclarationStmtNode node = new VarDeclarationStmtNode(parent, CurrentLine);
            int varDeclLine = CurrentLine;

            node.VarType = ParseType(); // string/int/ etc...
            node.VarName = ParseName(); // varname

            //check if already defined
            if (node.VarName != null && parent.IsNameIn(node.VarName))
                Log.Add(new ParserError("Variable is already defined in this context (" + node.VarName + ")",
                    varDeclLine, ThrowedIn.SpecifiedNodeParse));

            //assign default value
            if (NextTokenIs(PolyTokenType.Delimiter, ";", true))
            {
                ParseStatementEnd(); // ;

                return node;
            }

            ParseEquals(); // =

            node.VarValue = ParseBinaryExpression(0, node); // 1+2

            ParseStatementEnd(); // ;

            //if assigning empty array -> make it of var type
            if (node.VarValue.Type.Name == PolyTypes.Array.Name &&
                    node.VarValue is ArrayLiteralNode && ((ArrayLiteralNode)node.VarValue).Values.Length == 0)
                node.VarValue.Type.Of(node.VarType.T);

            //check if failed parsing value
            if (node.VarValue == null)
                Log.Add(new ParserError("Variable value is unknown",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check types for mismatch
            else if (!node.IsTypesValid())
                Log.Add(new ParserError($"Cannot assign value of type {node.VarValue.Type} to variable of type {node.VarType}",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check if variable already declared
            else if (parent.IsNameIn(node.VarName))
                Log.Add(new ParserError("Variable is already defined in this context (" +node.VarName+ ")",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return node;
        }
        //<NAME> = <EXPRESSION>;
        private VarAssignStmtNode ParseVarAssignation(AstNode parent)
        {
            StepInfo("Parsing Var Assignation");
            VarAssignStmtNode node = new VarAssignStmtNode(parent, CurrentLine);

            node.VarName = ParseName();

            ParseEquals();
            
            node.VarValue = ParseBinaryExpression(0, node);

            ParseStatementEnd();

            PolyType varType = parent.GetNameType(node.VarName);

            //if assigning empty array -> make it of var type
            if (varType.Name == PolyTypes.Array.Name&&
                    node.VarValue is ArrayLiteralNode && ((ArrayLiteralNode)node.VarValue).Values.Length == 0)
                node.VarValue.Type.Of(varType.T);

            //check if failed parsing value
            if (node.VarValue == null)
                Log.Add(new ParserError("Variable value is unknown",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check types for mismatch
            else if (varType != node.VarValue.Type)
                Log.Add(new ParserError($"Cannot assign value of type {node.VarValue.Type} to variable of type {parent.GetNameType(node.VarName)}",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check if not defined
            else if(!node.IsNameIn(node.VarName))
                Log.Add(new ParserError($"Cannot assign value, variable '{node.VarName}' not defined in current context",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check if constant
            else if (node.GetIsConstant(node.VarName))
                Log.Add(new ParserError($"Cannot assign value, variable '{node.VarName}' was constant",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return node;
        }

        #endregion

        #region Parse Method
        //method <TYPENAME>? <NAME> (<ARGS>) { <ALLOWED-NODES> }
        private MethodNode ParseMethod(AstNode parent)
        {
            StepInfo("Parsing Method");

            MethodNode methnode = new MethodNode(parent, CurrentLine);

            //return type
            PolyToken next = NextToken();
            //type exists?
            if (PolyTypes.FromName(next.Value) != PolyTypes.Unknown)
                methnode.MethodReturnType = PolyTypes.FromName(next.Value);
            //type not exists, type was not specified
            else
            {
                methnode.MethodReturnType = PolyTypes.Void;
                PushToken(next);
            }
            int methodDeclLine = CurrentLine;

            //name
            methnode.MethodName = ParseName();

            //args
            methnode.MethodArgs = ParseDefArgs(methnode);

            //body
            ParseCodeblockStart(); // {

            //parse nodes until '}'
            for (var node = this.ParseNode(methnode, "}");
                node != null;
                node = this.ParseNode(methnode, "}"))
            {
                //declare var OR etc
                if (node is UnknownNode == false)
                {
                    methnode.Childs.Add(node);
                }
            }

            ParseCodeblockEnd(); // }

            //TODO: better algorithm for checking returns:
            //      if current block has return -> childs not required to have return
            //
            //      example: -> NO ERRORS
            //
            //      method int a() {
            //          if(100 == 99 + 1) {}
            //          return 0;
            //      }

            //check if not returns
            if(methnode.MethodReturnType != PolyTypes.Void && !methnode.IsAllCodePathsReturns())
                Log.Add(new ParserError($"Not all code paths returns in method '{methnode.MethodName}'",
                    methodDeclLine, ThrowedIn.SpecifiedNodeParse));
            //check if method already defined
            else if (parent.IsNameIn(methnode.MethodName))
                Log.Add(new ParserError($"Name '{methnode.MethodName}' is already defined in this context",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return methnode;
        }
        #endregion

        #region Parse Condition
        //if(<EXPRESSION>) { <ALLOWED-NODES> }
        private IfNode ParseIf(AstNode parent)
        {
            StepInfo("Parsing Method");

            IfNode ifnode = new IfNode(parent, CurrentLine);

            int ifDeclLine = CurrentLine;

            ParseArgStart(); // (
            ExpressionNode expr = ParseBinaryExpression(0, ifnode);
            ParseArgEnd(); // )

            ifnode.Condition = expr;

            //body
            ParseCodeblockStart(); // {

            //parse nodes until '}'
            for (var node = this.ParseNode(ifnode, "}");
                node != null;
                node = this.ParseNode(ifnode, "}"))
            {
                //declare var OR etc
                if (node is UnknownNode == false)
                    ifnode.Childs.Add(node);
            }

            ParseCodeblockEnd(); // }

            //check if not returns bool
            if (expr.Type != PolyTypes.Bool)
                Log.Add(new ParserError($"Condition was of type {expr.Type} instead of bool type",
                    ifDeclLine, ThrowedIn.SpecifiedNodeParse));

            //TODO: parse elseif, else after

            return ifnode;
        }
        //else if(<EXPRESSION>) { <ALLOWED-NODES> }
        private ElseIfNode ParseElseIf(AstNode parent)
        {
            return null;
        }
        //else { <ALLOWED-NODES> }
        private ElseNode ParseElse(AstNode parent)
        {
            return null;
        }

        #endregion

        //TODO:
        //      ParseWhileBlock()

        //----------------------------------------------------------------------------
        // PARSE SPECIFIED NODE METHODS
        //----------------------------------------------------------------------------

        #region Parse Token
        /// <summary>
        /// Expect ';'
        /// </summary>
        private void ParseStatementEnd()
        {
            if(TryParseToken(PolyTokenType.Delimiter, ";", true) == false)
            {
                Log.Add(new ParserError("Expected end of statement", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '='
        /// </summary>
        private void ParseEquals()
        {
            if (TryParseToken(PolyTokenType.Operator, "=", true) == false)
            {
                Log.Add(new ParserError("Expected '='", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '{'
        /// </summary>
        private void ParseCodeblockStart()
        {
            if (TryParseToken(PolyTokenType.Delimiter, "{",false)==false)
            {
                Log.Add(new ParserError("Expected start of block", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '}'
        /// </summary>
        private void ParseCodeblockEnd()
        {
            if (TryParseToken(PolyTokenType.Delimiter, "}",false)==false)
            {
                Log.Add(new ParserError("Expected end of block", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '('
        /// </summary>
        private void ParseArgStart()
        {
            if (TryParseToken(PolyTokenType.Delimiter, "(", false) == false)
            {
                Log.Add(new ParserError("Expected '('", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect ')'
        /// </summary>
        private void ParseArgEnd()
        {
            if (TryParseToken(PolyTokenType.Delimiter, ")", false) == false)
            {
                Log.Add(new ParserError("Expected ')'", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect ','
        /// </summary>
        private void ParseComma()
        {
            if (TryParseToken(PolyTokenType.Delimiter, ",", false) == false)
            {
                Log.Add(new ParserError("Expected ','", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '['
        /// </summary>
        private void ParseIndexStart()
        {
            if (TryParseToken(PolyTokenType.Delimiter, "[",true)==false)
            {
                Log.Add(new ParserError("Expected '['", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect ']'
        /// </summary>
        private void ParseIndexEnd()
        {
            if (TryParseToken(PolyTokenType.Delimiter, "]",true)==false)
            {
                Log.Add(new ParserError("Expected ']'", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect ':'
        /// </summary>
        private void ParseColon()
        {
            if (TryParseToken(PolyTokenType.Delimiter, ":", true) == false)
            {
                Log.Add(new ParserError("Expected ':'", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '<'
        /// </summary>
        private void ParseTemplateStart()
        {
            if (TryParseToken(PolyTokenType.Operator, "<", true) == false)
            {
                Log.Add(new ParserError("Expected start of type template", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        /// <summary>
        /// Expect '>'
        /// </summary>
        private void ParseTemplateEnd()
        {
            if (TryParseToken(PolyTokenType.Operator, ">", true) == false)
            {
                Log.Add(new ParserError("Expected end of type template", CurrentLine,
                    ThrowedIn.SpecifiedTokenParse));
            }
            else
                StepSuccess();
        }
        #endregion

        #region Parse Expression
        /// <summary>
        /// Parses binary expression
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private ExpressionNode ParseBinaryExpression(int level, AstNode parent)
        {
            if (level >= BinaryOps.Length)
                return this.ParseTerm(parent);

            ExpressionNode lval = this.ParseBinaryExpression(level + 1, parent);
            if (lval == null)
                return null;

            PolyToken token;
            for (token = this.NextToken(); token != null && this.IsBinaryOperator(level, token);
                token = this.NextToken())
            {
                StepPrint(token.Value);

                switch (token.Value)
                {
                    //arithmetic operators
                    case "+":
                        AddExpressionNode addnode = new AddExpressionNode(parent, CurrentLine);
                        addnode.Left = lval;
                        ExpressionNode add_rval = this.ParseBinaryExpression(level + 1, addnode);
                        addnode.Right = add_rval;

                        lval = addnode;
                        //check types
                        if (!addnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '+' cannot be applied to operands of type " +
                                " '" + addnode.Left.Type.Name + "' and '" + addnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "-":
                        SubtractExpressionNode subnode = new SubtractExpressionNode(parent, CurrentLine);
                        subnode.Left = lval;
                        ExpressionNode sub_rval = this.ParseBinaryExpression(level + 1, subnode);
                        subnode.Right = sub_rval;

                        lval = subnode;
                        //check types
                        if (!subnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '-' cannot be applied to operands of type " +
                                " '" + subnode.Left.Type.Name + "' and '" + subnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "*":
                        MultiplyExpressionNode mulnode = new MultiplyExpressionNode(parent, CurrentLine);
                        mulnode.Left = lval;
                        ExpressionNode mul_rval = this.ParseBinaryExpression(level + 1, mulnode);
                        mulnode.Right = mul_rval;

                        lval = mulnode;
                        //check types
                        if (!mulnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '*' cannot be applied to operands of type " +
                                " '" + mulnode.Left.Type.Name + "' and '" + mulnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "/":
                        DivideExpressionNode divnode = new DivideExpressionNode(parent, CurrentLine);
                        divnode.Left = lval;
                        ExpressionNode div_rval = this.ParseBinaryExpression(level + 1, divnode);
                        divnode.Right = div_rval;

                        lval = divnode;
                        //check types
                        if (!divnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '/' cannot be applied to operands of type " +
                                " '" + divnode.Left.Type.Name + "' and '" + divnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        //check for dividing by 0 error
                        else if(div_rval is IntLiteralNode && ((IntLiteralNode)div_rval).Value == 0)
                            Log.Add(new ParserError("Cannot divide by zero",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "%":
                        ModulusExpressionNode modnode = new ModulusExpressionNode(parent, CurrentLine);
                        modnode.Left = lval;
                        ExpressionNode mod_rval = this.ParseBinaryExpression(level + 1, modnode);
                        modnode.Right = mod_rval;

                        lval = modnode;
                        //check types
                        if (!modnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '%' cannot be applied to operands of type " +
                                " '" + modnode.Left.Type.Name + "' and '" + modnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    //boolean operators
                    case "==":
                        EqualsExpressionNode eqnode = new EqualsExpressionNode(parent, CurrentLine);
                        eqnode.Left = lval;
                        ExpressionNode eq_rval = this.ParseBinaryExpression(level + 1, eqnode);
                        eqnode.Right = eq_rval;

                        lval = eqnode;
                        //check types
                        if (!eqnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '==' cannot be applied to operands of type " +
                                " '" + eqnode.Left.Type.Name + "' and '" + eqnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "!=":
                        NotEqualsExpressionNode neqnode = new NotEqualsExpressionNode(parent, CurrentLine);
                        neqnode.Left = lval;
                        ExpressionNode neq_rval = this.ParseBinaryExpression(level + 1, neqnode);
                        neqnode.Right = neq_rval;

                        lval = neqnode;
                        //check types
                        if (!neqnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '!=' cannot be applied to operands of type " +
                                " '" + neqnode.Left.Type.Name + "' and '" + neqnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "<":
                        LessThanExpressionNode lesstnode = new LessThanExpressionNode(parent, CurrentLine);
                        lesstnode.Left = lval;
                        ExpressionNode lesst_rval = this.ParseBinaryExpression(level + 1, lesstnode);
                        lesstnode.Right = lesst_rval;

                        lval = lesstnode;
                        //check types
                        if (!lesstnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '<' cannot be applied to operands of type " +
                                " '" +  lesstnode.Left.Type.Name + "' and '" + lesstnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "<=":
                        LessThanOrEqualsExpressionNode lesseqnode = new LessThanOrEqualsExpressionNode(parent, CurrentLine);
                        lesseqnode.Left = lval;
                        ExpressionNode lesseq_rval = this.ParseBinaryExpression(level + 1, lesseqnode);
                        lesseqnode.Right = lesseq_rval;

                        lval = lesseqnode;
                        //check types
                        if (!lesseqnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '<=' cannot be applied to operands of type " +
                                " '" + lesseqnode.Left.Type.Name + "' and '" + lesseqnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case ">":
                        MoreThanExpressionNode moretnode = new MoreThanExpressionNode(parent, CurrentLine);
                        moretnode.Left = lval;
                        ExpressionNode moret_rval = this.ParseBinaryExpression(level + 1, moretnode);
                        moretnode.Right = moret_rval;

                        lval = moretnode;
                        //check types
                        if (!moretnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '>' cannot be applied to operands of type " +
                                " '" + moretnode.Left.Type.Name + "' and '" + moretnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case ">=":
                        MoreThanOrEqualsExpressionNode moreeqnode = new MoreThanOrEqualsExpressionNode(parent, CurrentLine);
                        moreeqnode.Left = lval;
                        ExpressionNode moreeq_rval = this.ParseBinaryExpression(level + 1, moreeqnode);
                        moreeqnode.Right = moreeq_rval;

                        lval = moreeqnode;
                        //check types
                        if (!moreeqnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '>=' cannot be applied to operands of type " +
                                " '" + moreeqnode.Left.Type.Name + "' and '" + moreeqnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    //logical operators
                    case "&&":
                        AndExpressionNode andnode = new AndExpressionNode(parent, CurrentLine);
                        andnode.Left = lval;
                        ExpressionNode and_rval = this.ParseBinaryExpression(level + 1, andnode);
                        andnode.Right = and_rval;

                        lval = andnode;
                        //check types
                        if (!andnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '&&' cannot be applied to operands of type " +
                                " '" + andnode.Left.Type.Name + "' and '" + andnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "||":
                        OrExpressionNode ornode = new OrExpressionNode(parent, CurrentLine);
                        ornode.Left = lval;
                        ExpressionNode or_rval = this.ParseBinaryExpression(level + 1, ornode);
                        ornode.Right = or_rval;

                        lval = ornode;
                        //check types
                        if (!ornode.IsTypesValid())
                            Log.Add(new ParserError("Operator '||' cannot be applied to operands of type " +
                                " '" + ornode.Left.Type.Name + "' and '" + ornode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    default:
                        Log.Add(new ParserError("Unimplemented Operator '" + token.Value + "'",
                            CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        break;
                }
            }

            if (token != null)
            {
                StepBack(token.Value);
                PushToken(token);
            }

            return lval;
        }
        private ExpressionNode ParseTerm(AstNode parent)
        {
            PolyToken token = NextToken();

            if (token == null)
                return null;
            else
                StepPrint(token.Value);

            //bool literal
            if (token.Type == PolyTokenType.Name && (token.Value == "true" || token.Value == "false"))
                return new BoolLiteralNode(parent, (bool)PolyTypes.LiteralToNative(token.Value), CurrentLine);
            //null literal
            else if (token.Type == PolyTokenType.Name && (token.Value == "null"))
                return new NullLiteralNode(parent, CurrentLine);
            //string literal
            else if (token.Type == PolyTokenType.String)
                return new StringLiteralNode(parent, token.Value, CurrentLine);
            //int literal
            else if (token.Type == PolyTokenType.Integer)
                return new IntLiteralNode(parent, int.Parse(token.Value, CultureInfo.InvariantCulture), CurrentLine);
            //real(double) val
            else if (token.Type == PolyTokenType.Real)
                return new RealLiteralNode(parent, double.Parse(token.Value, CultureInfo.InvariantCulture), CurrentLine);
            //array val
            else if (token.Value == "[")
            {
                List<ExpressionNode> items = ParseExpressionList(parent, "]"); // expr, expr... ]

                //checking if all items type is same
                PolyType prevType = null;
                foreach(ExpressionNode item in items)
                {
                    //assign first previous
                    if (prevType is null)
                        prevType = item.Type;
                    //types not same
                    else if (item.Type != prevType)
                    {
                        Log.Add(new ParserError($"Array types mismatch, expected {prevType.Name} but got {item.Type.Name}", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                    }
                }

                return new ArrayLiteralNode(null, prevType is null ? PolyTypes.Unknown : prevType, items.ToArray(), CurrentLine);
            }
            //var/method call/index
            else if (token.Type == PolyTokenType.Name)
            {
                PolyToken next = NextToken();

                //method call
                if (next.Type == PolyTokenType.SpecialCharacter && next.Value == "(")
                {
                    if (parent.IsNameIn(token.Value))
                    {
                        if (parent.GetNameType(token.Value) == PolyTypes.Function)
                        {
                            //get args
                            List<ExpressionNode> args = ParseExpressionList(parent, ")");

                            return new MethodCallNode(parent, token.Value, parent.GetMethodType(token.Value), args, CurrentLine);
                        }
                        else
                        {
                            Log.Add(new ParserError($"Cannot call method '{token.Value}', value was not method", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                            return new VarNameNode(parent, "_", PolyTypes.Unknown, CurrentLine);
                        }
                    }
                    else
                    {
                        Log.Add(new ParserError($"Not found method in current scope ('{token.Value}')", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        return new VarNameNode(parent, "_", PolyTypes.Unknown, CurrentLine);
                    }
                }
                //index
                if (next.Type == PolyTokenType.SpecialCharacter && next.Value == "[")
                {
                    if (parent.IsNameIn(token.Value))
                    {
                        PolyType enumerableType = parent.GetNameType(token.Value);

                        //its enumerable
                        if (enumerableType.IsEnumerable)
                        {
                            PushToken(next); //push back '['

                            //get index
                            ExpressionNode index = ParseIndex(parent);

                            return new ArrayIndexNode(parent, token.Value, enumerableType.T, index, CurrentLine);
                        }
                        else
                        {
                            Log.Add(new ParserError($"Cannot get value by index, value of '{token.Value}' was not enumerable", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                            return new VarNameNode(parent, "_", PolyTypes.Unknown, CurrentLine);
                        }
                    }
                    else
                    {
                        Log.Add(new ParserError($"Not found enumerable in current scope ('{token.Value}')", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        return new VarNameNode(parent, "_", PolyTypes.Unknown, CurrentLine);
                    }
                }
                //var
                else
                {
                    if (parent.IsNameIn(token.Value))
                    {

                        PushToken(next); //push back after checking for method call
                        return new VarNameNode(parent, token.Value, parent.GetNameType(token.Value), CurrentLine);
                    }
                    else
                    {
                        Log.Add(new ParserError($"Not found variable in current scope ('{token.Value}')", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        return new VarNameNode(parent, "_", PolyTypes.Unknown, CurrentLine);
                    }
                }
            }

            this.PushToken(token);

            return null;
        }
        /// <summary>
        /// Parses boolean expression
        /// </summary>
        /// <returns></returns>
        private ExpressionNode ParseBooleanExpressionNode(AstNode parent)
        {
            int exprLine = CurrentLine;

            //parse expression
            ExpressionNode exprNode = ParseBinaryExpression(0,parent);
            //check expression type if its invalid
            if (exprNode.Type != PolyTypes.Bool)
                Log.Add(new ParserError("Expected boolean expression", exprLine, ThrowedIn.SpecifiedNodeParse));

            return exprNode;
        }
        /// <summary>
        /// Check if token is binary operator
        /// </summary>
        /// <param name="level"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsBinaryOperator(int level, PolyToken token)
        {
            return token.Type == PolyTokenType.Operator && BinaryOps[level].Contains(token.Value);
        }
        #endregion

        #region Parse Other
        /// <summary>
        /// Parse arguments
        /// <para>Example: int a, int b, string c</para>
        /// </summary>
        /// <param name="statementAfter"></param>
        /// <returns></returns>
        private Dictionary<string, PolyType> ParseDefArgs(AstNode parent)
        {
            // '('
            ParseArgStart();

            //parse loop until ')'
            Dictionary<string, PolyType> args = new Dictionary<string, PolyType>();
            bool previousWasArg = false;
            PolyToken token = null;
            for(token = NextToken(); token != null && token.Value != ")"; token = NextToken())
            {
                //arg
                if(token.Type == PolyTokenType.Name)
                {
                    if (previousWasArg == false)
                    {
                        PushToken(token);

                        string name = "_";
                        PolyType type = PolyTypes.Unknown;

                        ParseDefArg(out name, out type);

                        if (name != null && type != null)
                            args.Add(name,type);
                        previousWasArg = true;
                    }
                    else
                        Log.Add(new ParserError("Expected ',' or ')'", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                }
                //args next
                else if(token.Type == PolyTokenType.Delimiter && token.Value == ",")
                {
                    if (previousWasArg == false)
                    {
                        StepErr(",");
                        Log.Add(new ParserError("Expected argument", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                    }
                    else
                        StepSuccess();
                    previousWasArg = false;
                }
                //unknown
                else
                    Log.Add(new ParserError($"Unknown or unexpected statement in arguments ('{token.Value}')", CurrentLine, ThrowedIn.SpecifiedNodeParse));
            }

            //last token not was ')'
            if (token.Value == null)
                Log.Add(new ParserError("Expected ')' after arguments definition", CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return args;
        }
        /// <summary>
        /// Parse one argument
        /// <para>Example: int a</para>
        /// </summary>
        /// <returns></returns>
        private void ParseDefArg(out string name,out PolyType type)
        {
            //parse type,name
            PolyType _type = ParseType();
            string _name = ParseName();

            if (_name != null && _type != null)
            {
                ActionSuccess("Parsed argument node");
                name = _name;
                type = _type;
            }
            else
            {
                ActionFail("parsing argument node");
                name = null;
                type = null;
            }
        }

        /// <summary>
        /// Parse arguments
        /// <para>Example: 'hello',somevar,1+2</para>
        /// </summary>
        private List<ExpressionNode> ParseExpressionList(AstNode parent, string until)
        {
            //parse loop until ')'
            List<ExpressionNode> args = new List<ExpressionNode>();
            bool previousWasArg = false;
            PolyToken token = null;
            for (token = NextToken(); token != null && token.Value != until; token = NextToken())
            {
                //args next
                if (token.Type == PolyTokenType.Delimiter && token.Value == ",")
                {
                    if (previousWasArg == false)
                    {
                        StepErr(",");
                        Log.Add(new ParserError("Expected expression", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                    }
                    else
                        StepSuccess();

                    previousWasArg = false;
                }
                //arg
                else
                {
                    if (previousWasArg == false)
                    {
                        PolyToken tempToken = token;
                        PushToken(token);

                        ExpressionNode expr = ParseBinaryExpression(0, parent);
                        //was expression
                        if (expr != null)
                            args.Add(expr);
                        //was not expression
                        else
                            Log.Add(new ParserError($"Unknown or unexpected statement in expression list('{tempToken.Value}')", CurrentLine, ThrowedIn.SpecifiedNodeParse));

                        previousWasArg = true;
                    }
                    else
                        Log.Add(new ParserError($"Expected ',' or '{until}'", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                }
            }

            //last token not was {until}
            if (token.Value == null)
                Log.Add(new ParserError($"Expected '{until}' after expressions", CurrentLine, ThrowedIn.SpecifiedNodeParse));


            return args;
        }

        /// <summary>
        /// Parse name node
        /// <para>Example: somename_123_name</para>
        /// </summary>
        /// <returns></returns>
        private string ParseName()
        {
            PolyToken token = this.NextToken();

            //if its not name
            if (token == null || token.Type != PolyTokenType.Name)
            {
                ActionFail("parsing name(got:"+token.Type.ToString()+")");
                Log.Add(new ParserError("Expected name", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                return null;
            }
            else
            {
                StepSuccess();
                ActionSuccess("Parsed name");
            }
            //return name
            return token.Value;
        }
        /// <summary>
        /// Parse type node
        /// <para>Example: int/string/bool/real/object</para>
        /// </summary>
        /// <returns></returns>
        private PolyType ParseType()
        {
            PolyToken token = this.NextToken();

            //if its not typename
            if (token == null || token.Type != PolyTokenType.Name)
            {
                ActionFail("Parsing type");
                Log.Add(new ParserError("Expected type", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                return null;
            }
            else
            {
                ActionSuccess("Parsed type");
                StepSuccess();
            }

            //typename
            PolyType type = PolyTypes.FromName(token.Value);

            PolyToken next = this.NextToken();

            //check if T specified
            if(next.Value == "<" && type.CanBeTemplated)
            {
                PolyType T = ParseType();
                type.Of(T);

                ParseTemplateEnd(); // >

                return type;
            }
            else
            {
                PushToken(next);
                return type;
            }
        }

        /// <summary>
        /// Parse index node
        /// <para>Example: [123]/[21321+12*1]</para>
        /// </summary>
        /// <returns></returns>
        private ExpressionNode ParseIndex(AstNode parent)
        {
            ParseIndexStart();
            ExpressionNode expr = ParseBinaryExpression(0, parent);
            ParseIndexEnd();

            return expr;
        }

        /// <summary>
        /// Parse static string node
        /// <para>Example: "hello"/'world'</para>
        /// </summary>
        /// <returns></returns>
        private string ParseStringLiteral(AstNode parent)
        {
            PolyToken token = this.NextToken();
            //if its not str
            if (token == null || token.Type != PolyTokenType.String)
            {
                ActionFail("parsing static string");
                Log.Add(new ParserError("Expected static string", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                return null;
            }
            else
            {
                ActionSuccess("Parsed static string");
                StepSuccess();
            }
            //return str
            return token.Value.Replace("\"","").Replace("'", "");
        }

        /// <summary>
        /// Parse static string node
        /// <para>Example: "hello"/'world'</para>
        /// </summary>
        /// <returns></returns>
        private ExpressionNode[] ParseArrayLiteral(AstNode parent)
        {
            // '('
            ParseIndexStart();

            //parse loop until ')'
            List<ExpressionNode> items = new List<ExpressionNode>();
            bool previousWasItem = false;
            PolyToken token = null;
            for (token = NextToken(); token != null && token.Value != "]"; token = NextToken())
            {
                //args next
                if (token.Type == PolyTokenType.Delimiter && token.Value == ",")
                {
                    if (previousWasItem == false)
                    {
                        StepErr(",");
                        Log.Add(new ParserError("Expected value", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                    }
                    else
                        StepSuccess();

                    previousWasItem = false;
                }
                //arg
                else
                {
                    if (previousWasItem == false)
                    {
                        PolyToken tempToken = token;
                        PushToken(token);

                        ExpressionNode expr = ParseBinaryExpression(0, parent);
                        //was expression
                        if (expr != null)
                            items.Add(expr);
                        //was not expression
                        else
                            Log.Add(new ParserError($"Unknown or unexpected statement in array('{tempToken.Value}')", CurrentLine, ThrowedIn.SpecifiedNodeParse));

                        previousWasItem = true;
                    }
                    else
                        Log.Add(new ParserError("Expected ',' or ']'", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                }
            }

            //last token not was ')'
            if (token.Value == null)
                Log.Add(new ParserError("Expected ']' after array", CurrentLine, ThrowedIn.SpecifiedNodeParse));


            return items.ToArray();
        }
        #endregion

        #region Log
        //log
        /// <summary>
        /// Token step back log
        /// </summary>
        /// <param name="tok"></param>
        private void StepBack(string tok)
        {
            if (DoStepLog)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Parser Step Back: '" + tok+"'");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Token step print log
        /// </summary>
        /// <param name="tok"></param>
        private void StepPrint(string tok)
        {
            if (DoStepLog)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Parser Step: '" + tok+"'");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Token step info log
        /// </summary>
        /// <param name="info"></param>
        private void StepInfo(string info)
        {
            if (DoStepLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Parser Step Info ({CurrentLine}): {info}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Token step error log
        /// </summary>
        /// <param name="tok"></param>
        private void StepErr(string tok)
        {
            if (DoStepLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Parser Step Error: '" + tok+"'");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Token step success log
        /// </summary>
        private void StepSuccess()
        {
            if (DoStepLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Step was success");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Action success log
        /// </summary>
        /// <param name="action">action</param>
        private void ActionSuccess(string action)
        {
            if (DoActionLog)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[ParserAction:Success] "+action);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Action failed log
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="abbreviation"></param>
        private void ActionFail(string action,string abbreviation="Failed ")
        {
            if (DoActionLog)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ParserAction:Fail] " + abbreviation + action);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        #endregion

        #region Tokens Utils
        /// <summary>
        /// Go next (between tokens)
        /// </summary>
        /// <param name="onThisLine">does token should be on current line</param>
        /// <returns>token</returns>
        private PolyToken NextToken(bool onThisLine=false)
        {
            //get token
            PolyToken value = null;
            if (this.Toks.Count > 0)
                value = this.Toks.Pop();
            else
                value = this.Lexer.NextToken();

            if(value!=null)
                StepPrint(value.Value.Replace("\n",@"\n").Replace("\r",@"\r"));

            //if its newline token
            if (value!=null && value.Type == PolyTokenType.NewLine && onThisLine==false)
            {
                StepSuccess();
                CurrentLine++;
                return NextToken();
            }

            return value;
        }
        /// <summary>
        /// Go back (between tokens)
        /// </summary>
        /// <param name="token">token to push back</param>
        private void PushToken(PolyToken token)
        {
            this.Toks.Push(token);
            if (token.Type == PolyTokenType.NewLine)
                CurrentLine--;

            StepBack(token.Value);
        }

        /// <summary>
        /// DONT USE: WILL CAUSE IN ERROR REPORTING BUGS
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        private void _ParseToken(PolyTokenType type, string value,string name=null)
        {
            PolyToken token = this.NextToken();

            if (name == null) //if not has name
                name = value; //name is value (example: ';')
            if (token == null || token.Type != type || token.Value != value)
            {
                if (token != null && token.Type == PolyTokenType.NewLine) //if its newline
                    PushToken(token); //push
                if (token == null)
                    token = new PolyToken("<EOF>", PolyTokenType.NewLine);

                Log.Add(new ParserError($"Expected {name}, but got '"+token.Value+"'", CurrentLine, ThrowedIn.TokenParse));
            }
        }

        /// <summary>
        /// Check next token (pushes back that token)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="onThisLine"></param>
        /// <returns></returns>
        private bool NextTokenIs(PolyTokenType type, string value, bool onThisLine)
        {
            PolyToken token = this.NextToken(onThisLine);

            if (token != null && token.Type != PolyTokenType.NewLine)
                this.PushToken(token);

            if (token != null && token.Type == type && token.Value == value)
                return true;

            if (token != null)
                StepInfo("TokenCheck not passed(value:" +
                    token.Type.ToString() + "/'" + token.Value + "',expected:" + type.ToString() + "/'" + value +
                    "')");

            return false;
        }
        /// <summary>
        /// Expect next token (without exception throwing)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="onThisLine"></param>
        /// <returns></returns>
        private bool TryParseToken(PolyTokenType type, string value, bool onThisLine)
        {
            PolyToken token = this.NextToken(onThisLine);

            if (token != null && token.Type == type && token.Value == value)
                return true;

            if(token != null && token.Type != PolyTokenType.NewLine)
                this.PushToken(token);

            if(token != null)
                StepInfo("TryParse not passed(value:" +
                    token.Type.ToString() + "/'" + token.Value + "',expected:" + type.ToString() + "/'" + value +
                    "')");

            return false;
        }
        #endregion
    }
}