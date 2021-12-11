using Interpreter.Lexers;
using Interpreter.SourceCodeReader;
using Interpreter.Tokens;
using System.Collections.Generic;
using Xunit;

namespace Tests.LexerTests
{
    public class LexerTest
    {
        [Theory]
        [InlineData("program", TokenType.Program)]
        [InlineData("class", TokenType.Class)]
        [InlineData("def", TokenType.Def)]
        [InlineData("int", TokenType.Int)]
        [InlineData("bool", TokenType.Bool)]
        [InlineData("false", TokenType.False)]
        [InlineData("true", TokenType.True)]
        [InlineData("void", TokenType.Void)]
        [InlineData("while", TokenType.While)]
        [InlineData("if", TokenType.If)]
        [InlineData("else", TokenType.Else)]
        [InlineData("return", TokenType.Return)]
        [InlineData("(", TokenType.RoundOpenBracket)]
        [InlineData(")", TokenType.RoundCloseBracker)]
        [InlineData("{", TokenType.CurlyOpenBracket)]
        [InlineData("}", TokenType.CurlyCloseBracket)]
        [InlineData("+", TokenType.Plus)]
        [InlineData("-", TokenType.Minus)]
        [InlineData("*", TokenType.Multiplication)]
        [InlineData("/", TokenType.Division)]
        [InlineData("%", TokenType.Modulo)]
        [InlineData("<", TokenType.Less)]
        [InlineData(">", TokenType.Grater)]
        [InlineData("==", TokenType.Equal)]
        [InlineData("!=", TokenType.NotEqual)]
        [InlineData("<=", TokenType.LessOrEqual)]
        [InlineData(">=", TokenType.GraterOrEqual)]
        [InlineData("and", TokenType.And)]
        [InlineData("or", TokenType.Or)]
        [InlineData("not", TokenType.Not)]
        [InlineData("1234", TokenType.IntLiteral)]
        [InlineData("\"abcd\"", TokenType.StringLiteral)]
        [InlineData("identifier", TokenType.Identifier)]
        [InlineData("=", TokenType.Assign)]
        [InlineData(";", TokenType.Semicolon)]
        [InlineData(",", TokenType.Comma)]
        [InlineData(".", TokenType.Dot)]

        public void SingleTokenTest(string sourceCode, TokenType expectedTokenType)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            Assert.Equal(lexer.GetNextToken().TokenType, expectedTokenType);
        }

        [Theory]
        [InlineData("program \n { \n int i = 3; \n }", 
            new[]
            { 
                TokenType.Program, 
                TokenType.CurlyOpenBracket, 
                TokenType.Int, TokenType.Identifier, TokenType.Assign, TokenType.IntLiteral, TokenType.Semicolon, 
                TokenType.CurlyCloseBracket, 
                TokenType.EndOfFile 
            })]
        [InlineData("program \r\n { \n bool t = 3 == 4; \n if (t) \n { \n print(\"prawda\"); \n } \n}",
            new[]
            {
                TokenType.Program,
                TokenType.CurlyOpenBracket,
                TokenType.Bool, TokenType.Identifier, TokenType.Assign, TokenType.IntLiteral, TokenType.Equal, TokenType.IntLiteral, TokenType.Semicolon,
                TokenType.If, TokenType.RoundOpenBracket, TokenType.Identifier, TokenType.RoundCloseBracker,
                TokenType.CurlyOpenBracket,
                TokenType.Identifier, TokenType.RoundOpenBracket, TokenType.StringLiteral, TokenType.RoundCloseBracker, TokenType.Semicolon,
                TokenType.CurlyCloseBracket,
                TokenType.CurlyCloseBracket,
                TokenType.EndOfFile
            })]
        [InlineData("program\n{\n#new comment\nclass Rectangle\n{\nint X;\nint Y;\ndef init(int x, int y)\n{\nX = x;\nY=y;\n}\n}\ndef main()\n{\n}\n}",
            new[]
            {
                TokenType.Program,
                TokenType.CurlyOpenBracket,
                TokenType.Class, TokenType.Identifier, 
                TokenType.CurlyOpenBracket, 
                TokenType.Int, TokenType.Identifier, TokenType.Semicolon,
                TokenType.Int, TokenType.Identifier, TokenType.Semicolon,
                TokenType.Def, TokenType.Identifier, TokenType.RoundOpenBracket, TokenType.Int, TokenType.Identifier, TokenType.Comma, TokenType.Int, TokenType.Identifier, TokenType.RoundCloseBracker,
                TokenType.CurlyOpenBracket,
                TokenType.Identifier, TokenType.Assign, TokenType.Identifier, TokenType.Semicolon,
                TokenType.Identifier, TokenType.Assign, TokenType.Identifier, TokenType.Semicolon,
                TokenType.CurlyCloseBracket,
                TokenType.CurlyCloseBracket,
                TokenType.Def, TokenType.Identifier, TokenType.RoundOpenBracket, TokenType.RoundCloseBracker,
                TokenType.CurlyOpenBracket,
                TokenType.CurlyCloseBracket,
                TokenType.CurlyCloseBracket,
                TokenType.EndOfFile
            })]

        public void ShortProgramTest(string sourceCode, TokenType[] expectedTokens)
        {
            var tokens = new List<TokenType>();
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var token = lexer.GetNextToken();
            while (token.TokenType != TokenType.EndOfFile)
            {
                tokens.Add(token.TokenType);
                token = lexer.GetNextToken();
            }
            tokens.Add(token.TokenType);

            Assert.Equal(tokens.Count, expectedTokens.Length);
            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(tokens[i], expectedTokens[i]);
            }
        }
    }
}