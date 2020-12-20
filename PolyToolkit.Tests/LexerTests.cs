using System;
using PolyToolkit.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyToolkit.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void LexNameTest()
        {
            PolyLexer lexer = new PolyLexer("a agfc_123" + '"' + "b 123 - ! ? . ;" + '"');

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual("a", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.Name, res1.Type);

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual("agfc_123", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.Name, res2.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexStringTest()
        {
            PolyLexer lexer = new PolyLexer("a " + '"' + "b 123 - ! ? . ;" + '"' + "'hello world'");

            lexer.NextToken(); // skip 'a'

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual("b 123 - ! ? . ;", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.String, res1.Type);

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual("hello world", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.String, res2.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexDelimiterTest()
        {
            PolyLexer lexer = new PolyLexer(". abc , " + '"' + "b 123 - ! ? . ;" + '"');

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual(".", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.Delimiter, res1.Type);

            lexer.NextToken(); // skip 'abc'

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual(",", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.Delimiter, res2.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexSpecialCharTest()
        {
            PolyLexer lexer = new PolyLexer("#f @ abc , " + '"' + "b 123 - ! ? . ;" + '"');

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual("#", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.SpecialCharacter, res1.Type);

            lexer.NextToken(); // skip 'f'

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual("@", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.SpecialCharacter, res2.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexNewLineTest()
        {
            PolyLexer lexer = new PolyLexer("a \n" + '"' + "b 123 - ! ? . ;" + '"');

            lexer.NextToken(); // skip 'a'

            PolyToken res = lexer.NextToken();
            //value
            Assert.AreEqual("\n", res.Value);
            //type
            Assert.AreEqual(PolyTokenType.NewLine, res.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexOperatorTest()
        {
            PolyLexer lexer = new PolyLexer("a + - *= ***-" + '"' + "b 123 - ! ? . ;" + '"');

            lexer.NextToken(); // skip 'a'

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual("+", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.Operator, res1.Type);

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual("-", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.Operator, res2.Type);

            PolyToken res3 = lexer.NextToken();
            //value
            Assert.AreEqual("*=", res3.Value);
            //type
            Assert.AreEqual(PolyTokenType.Operator, res3.Type);

            PolyToken res4 = lexer.NextToken();
            //value
            Assert.AreEqual("***-", res4.Value);
            //type
            Assert.AreEqual(PolyTokenType.Operator, res4.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexIntegerTest()
        {
            PolyLexer lexer = new PolyLexer("a 321 54251 f " + '"' + "b 123 - ! ? . ;" + '"');

            lexer.NextToken(); //skip 'a'

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual("321", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.Integer, res1.Type);

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual("54251", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.Integer, res2.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }

        [TestMethod]
        public void LexRealTest()
        {
            PolyLexer lexer = new PolyLexer("a 321.21 54251.0f " + '"' + "b 123 - ! ? . ;" + '"');

            lexer.NextToken(); //skip 'a'

            PolyToken res1 = lexer.NextToken();
            //value
            Assert.AreEqual("321.21", res1.Value);
            //type
            Assert.AreEqual(PolyTokenType.Real, res1.Type);

            PolyToken res2 = lexer.NextToken();
            //value
            Assert.AreEqual("54251.0f", res2.Value);
            //type
            Assert.AreEqual(PolyTokenType.Real, res2.Type);

            //errors
            Assert.AreEqual(0, lexer.Log.Errors.Count);
        }
    }
}
