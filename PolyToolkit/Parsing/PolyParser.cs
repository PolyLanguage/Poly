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
            CurrentTree = new CodeTree(new List<IAstNode>());

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
                        if (CurrentTree.IsContainsNode<NamespaceStmtNode>() == false)
                            CurrentTree.Childs.Add(node);
                        else
                            Log.Add(new ParserError("Unexpected namespace statement", CurrentLine,
                                "You cannot define namespace second time.", ThrowedIn.CodeTreeParse));
                    }
                }
            }

            //if not contains namespace definition
            if (!CurrentTree.IsContainsNode<NamespaceStmtNode>())
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
        private IAstNode ParseNode(IWithBody parent,string until)
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
            #region Statement
            //var declaration (<TYPE> <NAME> = <EXPRESSION>;)
            else if (token.Type == PolyTokenType.Name && PolyType.IsItTypeName(token.Value))
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
                parent.IsVariableAvailable(token.Value))
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
            //unknown
            else
                Log.Add(new ParserError("Unknown or unexpected statement('" + token.Value + "')", CurrentLine, ThrowedIn.NodeParse));

            return new UnknownNode(parent);
        }
        #endregion

        #region Parse Statement
        //import <STATIC_STR>;
        private ImportStmtNode ParseImportStmt(IAstNode parent)
        {
            //example: import: <STATIC_STR>;
            ImportStmtNode node = new ImportStmtNode(parent);

            node.ImportValue = ParseStringLiteral(node);
            ParseStatementEnd();

            return node;
        }
        //namespace <STATIC_STR>;
        private NamespaceStmtNode ParseNamespaceStmt(IAstNode parent)
        {
            //example: namespace <STATIC_STR>;
            NamespaceStmtNode node = new NamespaceStmtNode(parent);

            node.NamespaceValue = ParseStringLiteral(node);
            ParseStatementEnd();

            return node;
        }

        //return <EXPR>;
        private ReturnStmtNode ParseReturnStmt(IAstNode parent)
        {
            //example: return <EXPR>;
            ReturnStmtNode node = new ReturnStmtNode(parent);

            node.ReturnValue = ParseBinaryExpression(0, node);
            ParseStatementEnd();

            return node;
        }
        #endregion

        #region Parse Class
        //class <NAME> { <ALLOWED-NODES> }
        private ClassNode ParseClass(IAstNode parent)
        {
            ClassNode clnode = new ClassNode(parent);

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
                if(node is UnknownNode==false)
                {
                    clnode.Childs.Add(node);
                }
            }

            //expect block end
            ParseCodeblockEnd();

            return clnode;
        }
        //ctor (<ARGS>) { <ALLOWED-NODES> }
        private ClassCtorNode ParseClassCtor(IAstNode parent)
        {
            //inst
            ClassCtorNode ctornode = new ClassCtorNode(parent);

            //identification
            ctornode.CtorArgs = ParseArgs(ctornode);

            ParseCodeblockStart(); // {

            //parse nodes until '}'
            for (var node = this.ParseNode(ctornode, "}");
                node != null;
                node = this.ParseNode(ctornode, "}"))
            {
                //declare var OR etc
                if (node is UnknownNode == false)
                {
                    ctornode.Childs.Add(node);
                }
            }

            ParseCodeblockEnd(); // }

            return ctornode;
        }
        #endregion

        #region Parse Variable
        //<TYPE> <NAME> = <EXPRESSION>; //TODO: ARRAYS DECLARATION, OBJECTS DECLARATION
        private VarDeclarationStmtNode ParseVarDeclaration(IAstNode parent)
        {
            VarDeclarationStmtNode node = new VarDeclarationStmtNode(parent);

            node.VarType = ParseType(); // string/int/ etc...
            node.VarName = ParseName(); //varname

            ParseEquals(); // =
            node.VarValue = ParseBinaryExpression(0, node); // 1+2
            ParseStatementEnd(); // ;

            //check if failed parsing value
            if(node.VarValue == null || node.VarValue.Type is null)
                Log.Add(new ParserError("Variable value is unknown",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check types for mismatch
            else if (!node.IsTypesValid())
                Log.Add(new ParserError("Type mismatch ("+node.VarType.Name+" & "+node.VarValue.Type.Name+")",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            //check if variable already declared
            if (parent.IsVariableAvailable(node.VarName))
                Log.Add(new ParserError("Variable is already defined in this context (" +node.VarName+ ")",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return node;
        }
        //<NAME> = <EXPRESSION>;
        private VarAssignStmtNode ParseVarAssignation(IAstNode parent)
        {
            VarAssignStmtNode node = new VarAssignStmtNode(parent);

            node.VarName = ParseName();

            ParseEquals();
            node.VarValue = ParseBinaryExpression(0, node);
            ParseStatementEnd();

            //check if failed parsing value
            if (node.VarValue == null || node.VarValue.Type is null)
                Log.Add(new ParserError("Variable value is unknown",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check types for mismatch
            else if (!node.IsTypesValid())
                Log.Add(new ParserError("Type mismatch (" + parent.GetVar(node.VarName).Name + " & " + node.VarValue.Type.Name + ")",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));

            return node;
        }

        #endregion

        #region Parse Method
        //method <TYPENAME> <NAME> (<ARGS>) { <ALLOWED-NODES> }
        private MethodNode ParseMethod(IAstNode parent)
        {
            MethodNode methnode = new MethodNode(parent);
            //identification
            methnode.MethodReturnType = ParseType();
            methnode.MethodName = ParseName();
            methnode.MethodArgs = ParseArgs(methnode);
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

            //TODO: check if return type exists (if required) AND check if return value not equals required return type

            return methnode;
        }
        #endregion

        //TODO: 
        //      ParseMethodCall(): <NAME>(<ARGS>);
        //      ParseIfBlock()
        //      ParseElseIfBlock()
        //      ParseElseBlock()
        //      ParseWhileBlock()
        //      ParseIncrement(): <NAME>++; | ++<NAME>;

        //----------------------------------------------------------------------------
        // PARSE SPECIFIED NODE METHODS
        //----------------------------------------------------------------------------

        #region Parse Token
        /// <summary>
        /// Expect ';'
        /// </summary>
        private void ParseStatementEnd()
        {
            if(TryParseToken(PolyTokenType.Delimiter, ";",true)==false)
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
            if (TryParseToken(PolyTokenType.Delimiter, ":",true) == false)
            {
                Log.Add(new ParserError("Expected ':'", CurrentLine,
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
        private IExpressionNode ParseBinaryExpression(int level, IAstNode parent)
        {
            if (level >= BinaryOps.Length)
                return this.ParseTerm(parent);

            IExpressionNode lval = this.ParseBinaryExpression(level + 1, parent);
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
                        AddExpressionNode addnode = new AddExpressionNode(parent);
                        addnode.Left = lval;
                        IExpressionNode add_rval = this.ParseBinaryExpression(level + 1, addnode);
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
                        SubtractExpressionNode subnode = new SubtractExpressionNode(parent);
                        subnode.Left = lval;
                        IExpressionNode sub_rval = this.ParseBinaryExpression(level + 1, subnode);
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
                        MultiplyExpressionNode mulnode = new MultiplyExpressionNode(parent);
                        mulnode.Left = lval;
                        IExpressionNode mul_rval = this.ParseBinaryExpression(level + 1, mulnode);
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
                        DivideExpressionNode divnode = new DivideExpressionNode(parent);
                        divnode.Left = lval;
                        IExpressionNode div_rval = this.ParseBinaryExpression(level + 1, divnode);
                        divnode.Right = div_rval;

                        lval = divnode;
                        //check types
                        if (!divnode.IsTypesValid())
                            Log.Add(new ParserError("Operator '/' cannot be applied to operands of type " +
                                " '" + divnode.Left.Type.Name + "' and '" + divnode.Right.Type.Name + "'",
                                CurrentLine, ThrowedIn.SpecifiedNodeParse));
                        else
                            StepSuccess();
                        break;
                    case "%":
                        ModulusExpressionNode modnode = new ModulusExpressionNode(parent);
                        modnode.Left = lval;
                        IExpressionNode mod_rval = this.ParseBinaryExpression(level + 1, modnode);
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
                        EqualsExpressionNode eqnode = new EqualsExpressionNode(parent);
                        eqnode.Left = lval;
                        IExpressionNode eq_rval = this.ParseBinaryExpression(level + 1, eqnode);
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
                        NotEqualsExpressionNode neqnode = new NotEqualsExpressionNode(parent);
                        neqnode.Left = lval;
                        IExpressionNode neq_rval = this.ParseBinaryExpression(level + 1, neqnode);
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
                        LessThanExpressionNode lesstnode = new LessThanExpressionNode(parent);
                        lesstnode.Left = lval;
                        IExpressionNode lesst_rval = this.ParseBinaryExpression(level + 1, lesstnode);
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
                        LessThanOrEqualsExpressionNode lesseqnode = new LessThanOrEqualsExpressionNode(parent);
                        lesseqnode.Left = lval;
                        IExpressionNode lesseq_rval = this.ParseBinaryExpression(level + 1, lesseqnode);
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
                        MoreThanExpressionNode moretnode = new MoreThanExpressionNode(parent);
                        moretnode.Left = lval;
                        IExpressionNode moret_rval = this.ParseBinaryExpression(level + 1, moretnode);
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
                        MoreThanOrEqualsExpressionNode moreeqnode = new MoreThanOrEqualsExpressionNode(parent);
                        moreeqnode.Left = lval;
                        IExpressionNode moreeq_rval = this.ParseBinaryExpression(level + 1, moreeqnode);
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
                }
            }

            if (token != null)
            {
                StepBack(token.Value);
                PushToken(token);
            }

            return lval;
        }
        private IExpressionNode ParseTerm(IAstNode parent)
        {
            PolyToken token = NextToken();

            if (token == null)
                return null;
            else
                StepPrint(token.Value);

            //bool literal
            if (token.Type == PolyTokenType.Name && (token.Value == "true" || token.Value == "false"))
                return new BoolLiteralNode(parent, (bool)PolyType.LiteralToNative(token.Value));
            //null literal
            else if (token.Type == PolyTokenType.Name && (token.Value == "null"))
                return new NullLiteralNode(parent);
            //string literal
            else if (token.Type == PolyTokenType.String)
                return new StringLiteralNode(parent, token.Value);
            //int literal
            else if (token.Type == PolyTokenType.Integer)
                return new IntLiteralNode(parent, int.Parse(token.Value, CultureInfo.InvariantCulture));
            //real(double) val
            else if (token.Type == PolyTokenType.Real)
                return new RealLiteralNode(parent, double.Parse(token.Value, CultureInfo.InvariantCulture));
            //var/method call val
            else if (token.Type == PolyTokenType.Name)
            {
                if (parent.IsVariableAvailable(token.Value))
                {
                    return new VarNameNode(parent, token.Value, parent.GetVar(token.Value));

                    //TODO: method call val
                }
                else
                    Log.Add(new ParserError("Unknown identifier", CurrentLine, ThrowedIn.SpecifiedNodeParse));
            }

            this.PushToken(token);

            return null;
        }
        /// <summary>
        /// Parses boolean expression
        /// </summary>
        /// <returns></returns>
        private IExpressionNode ParseBooleanExpressionNode(IAstNode parent)
        {
            int exprLine = CurrentLine;

            //parse expression
            IExpressionNode exprNode = ParseBinaryExpression(0,parent);
            //check expression type if its invalid
            if (exprNode.Type != PolyType.BooleanType)
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
        /// Parse arguments (not parses brackets)
        /// <para>Example: int a, int b, string c</para>
        /// </summary>
        /// <param name="statementAfter"></param>
        /// <returns></returns>
        private Dictionary<string,PolyType> ParseArgs(IAstNode parent,bool statementAfter=false)
        {
            // '('
            ParseArgStart();

            //parse loop until ')'
            Dictionary<string, PolyType> args = new Dictionary<string, PolyType>();
            bool previousWasArg = false;
            for(var node = NextToken();node != null && node.Value!=")";node = NextToken())
            {
                //arg
                if(node.Type == PolyTokenType.Name)
                {
                    if (previousWasArg == false)
                    {
                        PushToken(node);

                        string name;
                        PolyType type;

                        ParseArg(out name,out type);

                        if (name != null && type != null)
                            args.Add(name,type);
                        previousWasArg = true;
                    }
                    else
                        Log.Add(new ParserError("Expected ',' or ')'", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                }
                //args next
                else if(node.Type == PolyTokenType.Delimiter && node.Value == ",")
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
                    Log.Add(new ParserError("Unknown or unexpected statement in arguments('"+node.Value+"')", CurrentLine, ThrowedIn.SpecifiedNodeParse));
            }

            //statement
            if(statementAfter)
                ParseStatementEnd();

            return args;
        }
        /// <summary>
        /// Parse one argument
        /// <para>Example: int a</para>
        /// </summary>
        /// <returns></returns>
        private void ParseArg(out string name,out PolyType type)
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
                ActionFail("parsing type");
                Log.Add(new ParserError("Expected type", CurrentLine, ThrowedIn.SpecifiedNodeParse));
                return null;
            }
            else
            {
                ActionSuccess("Parsed type");
                StepSuccess();
            }
            //return typename
            return PolyType.FromVarName(token.Value);
        }
        /// <summary>
        /// Parse index node
        /// <para>Example: [123]/[21321+12*1]</para>
        /// </summary>
        /// <returns></returns>
        private IExpressionNode ParseIndex(IAstNode parent)
        {
            ParseIndexStart();
            IExpressionNode expr = ParseBinaryExpression(0,parent);
            ParseIndexEnd();

            return expr;
        }

        /// <summary>
        /// Parse static string node
        /// <para>Example: "hello"/'world'</para>
        /// </summary>
        /// <returns></returns>
        private StringLiteralNode ParseStringLiteral(IAstNode parent)
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
            return new StringLiteralNode(parent,token.Value.Replace('"'.ToString(),""));
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
                Console.WriteLine("Parser Step Info: " + info);
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
        private bool NextTokenIs(PolyTokenType type,string value,bool onThisLine)
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
        private bool TryParseToken(PolyTokenType type, string value,bool onThisLine)
        {
            PolyToken token = this.NextToken(onThisLine);

            if (token != null && token.Type == type && token.Value == value)
                return true;

            if(token != null && token.Type != PolyTokenType.NewLine)
                this.PushToken(token);

            if(token!=null)
                StepInfo("TryParse not passed(value:" +
                    token.Type.ToString() + "/'" + token.Value + "',expected:" + type.ToString() + "/'" + value +
                    "')");

            return false;
        }
        #endregion
    }
}
