using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using PolyToolkit.Parsing.Ast;
using PolyToolkit.Evaluation;
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

        /// <summary>
        /// Parse code
        /// </summary>
        /// <returns></returns>
        public CodeTree ParseCode()
        {
            CurrentTree = new CodeTree(new List<IAstNode>());
            #region Parse Code

            for (var node = this.ParseNode(CurrentTree, null); node != null;
                node = this.ParseNode(CurrentTree, null))
            {
                //its import stmt or class
                if (node is UnknownNode==false)
                {
                    if(node is NamespaceStatementNode==false)
                        CurrentTree.Body.Childs.Add(node);
                    else
                    {
                        if (CurrentTree.IsContainsNamespaceNode() == false)
                            CurrentTree.Body.Childs.Add(node);
                        else
                            Log.Add(new ParserError("Unexpected namespace statement", CurrentLine,
                                "You cannot define namespace second time.", ThrowedIn.CodeTreeParse));
                    }
                }
            }
            #endregion

            //if not contains namespace definition
            if (!CurrentTree.IsContainsNamespaceNode())
                Log.Add(new ParserError("Undefined namespace", 1, ThrowedIn.CodeTreeParse));

            if (Log.Errors.Count == 0 && Lexer.Log.Errors.Count == 0)
                return CurrentTree; //return tree & attach context
            else
                return null;
        }

        #region Parse Core

        //parse current node
        private IAstNode ParseNode(IWithBody parent,string until)
        {
            PolyToken token = this.NextToken();

            if (token == null)
                return null;

            //class
            if (token.Type == PolyTokenType.Name && token.Value == "class")
            {
                if (parent.IsAllowed<ClassNode>())
                {
                    StepSuccess();
                    return ParseClass(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected class declaration", CurrentLine, ThrowedIn.NodeParse));
            }
            //import
            else if (token.Type == PolyTokenType.Name && token.Value == "import")
            {
                if (parent.IsAllowed<ImportStatementNode>())
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
                if (parent.IsAllowed<NamespaceStatementNode>())
                {
                    StepSuccess();
                    return ParseNamespaceStmt(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected namespace statement", CurrentLine, ThrowedIn.NodeParse));
            }
            //var declaration (<TYPE> <NAME> = <EXPRESSION>;)
            else if (token.Type == PolyTokenType.Name && PolyType.IsItTypeName(token.Value))
            {
                if (parent.IsAllowed<VarDeclarationNode>())
                {
                    PushToken(token);
                    return ParseVarDeclaration(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected declaration", CurrentLine, ThrowedIn.NodeParse));
            }
            //var assign (<NAME> = <EXPRESSION>;)
            else if(token.Type == PolyTokenType.Name && 
                CurrentTree.IsVariableAvailable(token.Value))
            {
                if (parent.IsAllowed<VarAssignNode>())
                {
                    PushToken(token);
                    return ParseVarAssignation(parent);
                }
                else
                    Log.Add(new ParserError("Unexpected assign", CurrentLine, ThrowedIn.NodeParse));
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
            //end
            else if (token.Value == until)
            {
                //push back
                PushToken(token);
                //return end
                return new EndOfBlockNode(parent);
            }
            //unknown
            else
                Log.Add(new ParserError("Unknown or unexpected statement('" + token.Value + "')", CurrentLine, ThrowedIn.NodeParse));

            return new UnknownNode(parent);
        }

        //import: <STATIC_STR>;
        private ImportStatementNode ParseImportStmt(IAstNode parent)
        {
            //example: import: <STATIC_STR>;
            ImportStatementNode node = new ImportStatementNode(parent);

            ParseColon();
            StaticStringNode importVal = ParseStaticString(node);
            ParseStatementEnd();

            node.ImportValue = importVal;
            return node;
        }
        //namespace: <STATIC_STR>;
        private NamespaceStatementNode ParseNamespaceStmt(IAstNode parent)
        {
            //example: namespace: <STATIC_STR>;
            NamespaceStatementNode node = new NamespaceStatementNode(parent);

            ParseColon();
            StaticStringNode nsVal = ParseStaticString(node);
            ParseStatementEnd();

            node.NamespaceValue = nsVal;
            return node;
        }
        #endregion

        #region Parse Class
        //class <NAME> { <ALLOWED-NODES> }
        private ClassNode ParseClass(IAstNode parent)
        {
            ClassNode clnode = new ClassNode(parent);

            //parse class name
            NameNode name = ParseName(clnode);

            //expect block start
            ParseCodeblockStart();

            //parse nodes until '}' (var/method)
            for (var node = this.ParseNode(clnode,"}");
                node != null && node.GetType() != typeof(EndOfBlockNode) ;
                node = this.ParseNode(clnode,"}"))
            {
                //not unknown
                if(node is UnknownNode==false)
                {
                    clnode.Body.Childs.Add(node);
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
            ArgumentsNode args = ParseArgs(ctornode);

            ParseCodeblockStart(); // {

            //parse nodes until '}'
            for (var node = this.ParseNode(ctornode, "}");
                node != null && node.GetType() != typeof(EndOfBlockNode);
                node = this.ParseNode(ctornode, "}"))
            {
                //declare var OR etc
                if (node is UnknownNode==false)
                {
                    ctornode.Body.Childs.Add(node);
                }
            }

            ParseCodeblockEnd(); // }

            return ctornode;
        }
        #endregion

        #region Parse Variable
        //<TYPE> <NAME> = <EXPRESSION>; //TODO: ARRAYS DECLARATION
        private VarDeclarationNode ParseVarDeclaration(IAstNode parent)
        {
            VarDeclarationNode node = new VarDeclarationNode(parent);

            TypenameNode type = ParseType(node); // string/int/ etc...
            NameNode name = ParseName(node); //varname
            ParseEquals(); // =
            IExpressionNode val = ParseExpressionNode(node); // 1+2
            ParseStatementEnd(); // ;

            //set value
            object objVal = null;
            if (val is ExpressionNode || val is NameNode)
                objVal = val.Expression;
            else if (val is ConstantNode)
                objVal = ((ConstantNode)val).Value;

            node.VarType = type;
            node.VarName = name;
            node.VarValue = val;

            //check types for mismatch
            if (!node.IsTypesValid())
                Log.Add(new ParserError("Type mismatch ("+type.Type.Name+" & "+val.Expression.Type.Name+")",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            //check if variable already declared
            if (CurrentTree.IsVariableAvailable(name.Value))
                Log.Add(new ParserError("Variable is already defined in this context (" +name.Value+ ")",
                    CurrentLine, ThrowedIn.SpecifiedNodeParse));
            
            return node;
        }
        //<NAME> = <EXPRESSION>;
        private VarAssignNode ParseVarAssignation(IAstNode parent)
        {
            Console.WriteLine("var assing");

            VarAssignNode node = new VarAssignNode(parent);

            NameNode name = ParseName(node);
            ParseEquals();
            IExpressionNode val = ParseExpressionNode(node);
            ParseStatementEnd();

            node.VarName = name;
            node.VarValue = val;
            return node;
        }

        #endregion

        #region Parse Method
        //method <TYPENAME> <NAME> (<ARGS>) { <ALLOWED-NODES> }
        private MethodNode ParseMethod(IAstNode parent)
        {
            MethodNode meth = new MethodNode(parent);
            //identification
            TypenameNode ret = ParseType(meth);
            NameNode name = ParseName(meth);
            ArgumentsNode args = ParseArgs(meth);
            //inst
            meth.MethodReturnType = ret;
            meth.MethodName = name;
            meth.MethodArgs = args;
            //body
            ParseCodeblockStart(); // {
            //TODO: Parse all nodes (with for loop)
            ParseCodeblockEnd(); // }

            return meth;
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
        private IExpression ParseBinaryExpression(int level)
        {
            if (level >= BinaryOps.Length)
                return this.ParseTerm();

            IExpression expr = this.ParseBinaryExpression(level + 1);
            if (expr == null)
                return null;

            PolyToken token;
            for (token = this.NextToken(); token != null && this.IsBinaryOperator(level, token);
                token = this.NextToken())
            {
                //arithmetic operators
                if (token.Value == "+")
                    expr = new AddExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "-")
                    expr = new SubtractExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "*")
                    expr = new MultiplyExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "/")
                    expr = new DivideExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "%")
                    expr = new ModulusExpression(expr, this.ParseBinaryExpression(level + 1));
                //boolean operators
                else if (token.Value == "==")
                    expr = new EqualExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "!=")
                    expr = new NotEqualExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "<")
                    expr = new LessThanExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == ">")
                    expr = new MoreThanExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == "<=")
                    expr = new LessThanOrEqualsExpression(expr, this.ParseBinaryExpression(level + 1));
                else if (token.Value == ">=")
                    expr = new MoreThanOrEqualsExpression(expr, this.ParseBinaryExpression(level + 1));
            }

            if (token != null)
                PushToken(token);
            return expr;
        }
        private IExpression ParseTerm()
        {
            PolyToken token = NextToken();

            if (token == null)
                return null;

            //literal
            if(token.Type == PolyTokenType.Name && PolyType.IsItLiteral(token.Value))
            {
                return new LiteralValueExpression(token.Value);
            }

            //var/method call val
            if (token.Type == PolyTokenType.Name)
            {
                return new VarExpression(token.Value);

                //TODO: metho call val
            }
            
            //string val
            if (token.Type == PolyTokenType.String)
                return new ConstExpression(token.Value);

            //int val
            if (token.Type == PolyTokenType.Integer)
                return new ConstExpression(int.Parse(token.Value, CultureInfo.InvariantCulture));

            //real(double) val
            if (token.Type == PolyTokenType.Real)
                return new ConstExpression(double.Parse(token.Value, CultureInfo.InvariantCulture));

            this.PushToken(token);

            return null;
        }
        /// <summary>
        /// Parses any expression (arithemic/boolean expression,constant) node
        /// </summary>
        /// <returns></returns>
        private IExpressionNode ParseExpressionNode(IAstNode parent)
        {
            IExpression expr = ParseBinaryExpression(0);

            if (expr == null)
                return null;
            else if (expr is ConstExpression)
                return new ConstantNode(parent,((ConstExpression)expr).ConstValue);
            else if(expr is VarExpression)
                return new NameNode(parent,((VarExpression)expr).VarName);
            //TODO: VarCall ^^^ instead of name

            return new ExpressionNode(parent,expr);
        }
        /// <summary>
        /// Parses boolean expression
        /// </summary>
        /// <returns></returns>
        private IExpressionNode ParseBooleanExpressionNode(IAstNode parent)
        {
            int exprLine = CurrentLine;

            //parse expression
            IExpressionNode exprNode = ParseExpressionNode(parent);
            //check expression type if its invalid
            if (exprNode.Expression.Type != PolyType.BooleanType)
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
        private ArgumentsNode ParseArgs(IAstNode parent,bool statementAfter=false)
        {
            // '('
            ParseArgStart();

            //parse loop until ')'
            ArgumentsNode args = new ArgumentsNode(parent);
            bool previousWasArg = false;
            for(var node = NextToken();node != null && node.Value!=")";node = NextToken())
            {
                //arg
                if(node.Type == PolyTokenType.Name)
                {
                    if (previousWasArg == false)
                    {
                        PushToken(node);
                        ArgumentNode arg = ParseArg(args);
                        if (arg != null)
                            args.Args.Add(arg);
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
        private ArgumentNode ParseArg(IAstNode parent)
        {
            ArgumentNode arg = new ArgumentNode(parent);
            //parse type,name
            TypenameNode type = ParseType(arg);
            NameNode name = ParseName(arg);

            //return
            if (name != null && type != null)
            {
                ActionSuccess("Parsed argument node");
                return arg;
            }

            ActionFail("parsing argument node");
            return null;
        }
        /// <summary>
        /// Parse name node
        /// <para>Example: somename_123_name</para>
        /// </summary>
        /// <returns></returns>
        private NameNode ParseName(IAstNode parent)
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
            return new NameNode(parent,token.Value);
        }
        /// <summary>
        /// Parse type node
        /// <para>Example: int/string/bool/real/object</para>
        /// </summary>
        /// <returns></returns>
        private TypenameNode ParseType(IAstNode parent)
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
            return new TypenameNode(parent,PolyType.FromVarName(token.Value));
        }
        /// <summary>
        /// Parse index node
        /// <para>Example: [123]/[21321+12*1]</para>
        /// </summary>
        /// <returns></returns>
        private IndexNode ParseIndex(IAstNode parent)
        {
            IndexNode node = new IndexNode(parent);
            ParseIndexStart();

            IExpressionNode expr = ParseExpressionNode(node);
            node.Index = expr;

            ParseIndexEnd();

            return node;
        }
        /// <summary>
        /// Parse static string node
        /// <para>Example: "hello"/'world'</para>
        /// </summary>
        /// <returns></returns>
        private StaticStringNode ParseStaticString(IAstNode parent)
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
            return new StaticStringNode(parent,token.Value.Replace('"'.ToString(),""));
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
