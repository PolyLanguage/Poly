using System;
using PolyToolkit;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyToolkit.Tests
{
    [TestClass]
    public class ParserTests
    {
        #region Core Statements
        [TestMethod]
        public void ParseImportNode()
        {
            string code = "import: 'System';\n"+
                "namespace: 'Test';";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(2, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[0] is ImportStatementNode);

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }

        [TestMethod]
        public void ParseNamespaceNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(2, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[1] is NamespaceStatementNode);

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }

        [TestMethod]
        public void ParseClassNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n"+
                "class Test {}";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }

        [TestMethod]
        public void ParseClassMethodNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { method null Test_123() {} }";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(1, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is MethodNode);
            //check method name
            Assert.AreEqual("Test_123",((MethodNode)res.Body.Childs[2].Childs[1].Childs[0]).MethodName.Value);
            Assert.AreEqual("null", ((MethodNode)res.Body.Childs[2].Childs[1].Childs[0]).MethodReturnType.Type.Name);
            
            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }

        [TestMethod]
        public void ParseMethodArgsNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { method null Test_123(int a,int b) {} }";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(1, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is MethodNode);
            //check method name
            Assert.AreEqual("Test_123", ((MethodNode)res.Body.Childs[2].Childs[1].Childs[0]).MethodName.Value);
            //check method return type
            Assert.AreEqual("null", ((MethodNode)res.Body.Childs[2].Childs[1].Childs[0]).MethodReturnType.Type.Name);
            //check args count
            Assert.AreEqual(2,((MethodNode)res.Body.Childs[2].Childs[1].Childs[0]).MethodArgs.Args.Count);

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }

        [TestMethod]
        public void ParseClassCtorNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { ctor() {} }";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(1, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is ClassCtorNode);

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }

        [TestMethod]
        public void ParseVarDeclarationNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { int a = 200; }";
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(1, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is VarDeclarationNode);

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }
        #endregion

        #region Expression
        [TestMethod]
        public void ParseSimpleExpressionNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { int a = 200+20; int c = 10+20-30+10*2/2; }"; //220,10
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
            
            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is VarDeclarationNode);
            //check result
            VarDeclarationNode decl = (VarDeclarationNode)res.Body.Childs[2].Childs[1].Childs[0];
            Assert.AreEqual(220, decl.VarValue.Expression.Evaluate(new Evaluation.DefaultContext()));

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[1] is VarDeclarationNode);
            //check result
            VarDeclarationNode decl2 = (VarDeclarationNode)res.Body.Childs[2].Childs[1].Childs[1];
            Assert.AreEqual(10, decl2.VarValue.Expression.Evaluate(new Evaluation.DefaultContext()));

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }
        [TestMethod]
        public void ParseBooleanExpressionNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { bool a = 20 == 20; bool c = 3213*321-200 <= 1; }"; //true,false
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is VarDeclarationNode);
            //check result
            VarDeclarationNode decl = (VarDeclarationNode)res.Body.Childs[2].Childs[1].Childs[0];
            Assert.AreEqual(true, decl.VarValue.Expression.Evaluate(new Evaluation.DefaultContext()));

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[1] is VarDeclarationNode);
            //check result
            VarDeclarationNode decl2 = (VarDeclarationNode)res.Body.Childs[2].Childs[1].Childs[1];
            Assert.AreEqual(false, decl2.VarValue.Expression.Evaluate(new Evaluation.DefaultContext()));

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }
        [TestMethod]
        public void ParseLiteralsExpressionNode()
        {
            string code = "import: 'System';\n" +
                "namespace: 'Test';\n" +
                "class Test { bool a = true; bool c = false; }"; //true,false
            PolyParser parser = new PolyParser(code);
            CodeTree res = parser.ParseCode();

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);

            //childs count
            Assert.AreEqual(3, res.Body.Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2] is ClassNode);

            //childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs.Count);

            //class name
            Assert.IsTrue(res.Body.Childs[2].Childs[0] is NameNode);
            //class body
            Assert.IsTrue(res.Body.Childs[2].Childs[1] is BodyNode);

            //body childs count
            Assert.AreEqual(2, res.Body.Childs[2].Childs[1].Childs.Count);

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[0] is VarDeclarationNode);
            //check result
            VarDeclarationNode decl = (VarDeclarationNode)res.Body.Childs[2].Childs[1].Childs[0];
            Assert.AreEqual(true, decl.VarValue.Expression.Evaluate(new Evaluation.DefaultContext()));

            //check type
            Assert.IsTrue(res.Body.Childs[2].Childs[1].Childs[1] is VarDeclarationNode);
            //check result
            VarDeclarationNode decl2 = (VarDeclarationNode)res.Body.Childs[2].Childs[1].Childs[1];
            Assert.AreEqual(false, decl2.VarValue.Expression.Evaluate(new Evaluation.DefaultContext()));

            //errors
            Assert.AreEqual(0, parser.Log.Errors.Count);
        }
        #endregion
    }
}
