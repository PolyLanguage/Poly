using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Debug;
namespace PolyToolkit.Parsing
{
    //lexer: reads words & chars and identifies token from every word/char
    public class PolyLexer
    {
        private static char[] Delimiters = new char[] 
        {
            ',','.',';',':',
            '(',')','{','}','[',']'
        };
        private static char[] Operators = new char[]
        {
            '-','+','*','/','%','=','>','<','!','?'
        };
        private static char[] SpecialChars = new char[]
        {
            '#',',','^','$','@','&','~'
        };

        public string Code { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public LexerLogger Log { get; }

        public PolyLexer(string code)
        {
            Code = code;
            Length = Code == null ? 0 : Code.Length;
            Log = new LexerLogger();
        }

        //next token
        public PolyToken NextToken()
        {
            this.SkipWhiteSpaces();

            if (Position >= Length)
                return null;

            char ch = Code[Position++];

            if (ch == '\r' || ch == '\t')
            {
                if (Position < Length && Code[Position] == '\n')
                {
                    Position++;
                    return new PolyToken("\r\n", PolyTokenType.NewLine);
                }

                return new PolyToken("\r", PolyTokenType.NewLine);
            }

            else if (ch == '\n')
                return new PolyToken("\n", PolyTokenType.NewLine); // \n

            else if (ch == '"' || ch == "'".ToCharArray().First())
                return this.NextString(ch); //string "a" or 'b'

            else if (IsOperator(ch))
                return this.NextOperator(ch); // + or - or += or etc

            else if (Delimiters.Contains(ch))
                return new PolyToken(ch.ToString(), PolyTokenType.Delimiter); //, or . or ( or etc

            else if (SpecialChars.Contains(ch))
                return new PolyToken(ch.ToString(), PolyTokenType.SpecialCharacter); // # or $ or etc

            else if (char.IsDigit(ch))
                return this.NextInteger(ch); // 123 or 1 or etc

            else if(char.IsLetter(ch))
                return this.NextName(ch); // abc or abc389021 or etc

            return null;
        }
        //string token: "string"
        private PolyToken NextString(char ch)
        {
            string value = string.Empty;

            while (Position < Length && Code[Position] != ch)
                value += Code[Position++];

            if (Position >= Length)
                Log.Add(new LexerError("Unclosed string"));

            Position++;

            return new PolyToken(value, PolyTokenType.String);
        }
        //operator token: +-*& etc
        private PolyToken NextOperator(char ch)
        {
            string value = ch.ToString();

            while (Position < Length)
            {
                var ch2 = Code[Position];

                if (!IsOperator(ch2))
                    break;

                value += ch2;
                Position++;
            }

            return new PolyToken(value, PolyTokenType.Operator);
        }
        //int token: 2184
        private PolyToken NextInteger(char ch)
        {
            string value = ch.ToString();

            while (Position < Length && char.IsDigit(Code[Position]))
                value += Code[Position++];

            if (Position < Length)
            {
                if (Code[Position] == '.')
                    return this.NextReal(value);

                if (char.IsLetter(Code[Position]))
                    Log.Add(new LexerError(string.Format("Unexpected '{0}' (IntegerLexer)", Code[Position])));
            }

            return new PolyToken(value, PolyTokenType.Integer);
        }
        //real token: 432.21 // 211.20f
        private PolyToken NextReal(string integer)
        {
            string value = integer + ".";
            Position++;

            while (Position < Length && char.IsDigit(Code[Position]))
                value += Code[Position++];

            //if last == digit
            if (Position < Length && !char.IsWhiteSpace(Code[Position]) && char.IsLetter(Code[Position]))
                value += Code[Position];
            //else if (!char.IsWhiteSpace(Code[Position]))
            //    Log.Add(new LexerError(string.Format("Unexpected '{0}' (RealLexer)", Code[Position])));

            return new PolyToken(value, PolyTokenType.Real);
        }
        //name token: somename
        private PolyToken NextName(char ch)
        {
            string value = ch.ToString();

            // '_' allowed
            while (Position < Length && (char.IsLetterOrDigit(Code[Position]) || Code[Position] == '_'))
            {
                value += Code[Position++];
            }

            return new PolyToken(value, PolyTokenType.Name);
        }

        private static bool IsWhiteSpace(char ch)
        {
            if (ch == '\r' || ch == '\n')
                return false;

            return char.IsWhiteSpace(ch);
        }
        private static bool IsOperator(char ch)
        {
            if (char.IsWhiteSpace(ch))
                return false;

            if (char.IsLetterOrDigit(ch))
                return false;

            if (Delimiters.Contains(ch))
                return false;

            if (Operators.Contains(ch))
                return true;

            return false;
        }

        private void SkipWhiteSpaces()
        {
            while (true)
            {
                while (Position < Length && IsWhiteSpace(Code[Position]))
                    Position++;

                if (Position >= Length)
                    return;

                if (Code[Position] == '/' && Position < Length - 1 && Code[Position + 1] == '/')
                {
                    while (Position < Length && Code[Position] != '\r' && Code[Position] != '\n')
                        Position++;
                }
                else
                    return;
            }
        }
    }
}
