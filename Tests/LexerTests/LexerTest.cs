using Interpreter.Lexers;
using Interpreter.SourceCodeReader;
using Interpreter.Tokens;
using Xunit;

namespace Tests.LexerTests
{
    public class LexerTest
    {
        [Theory]
        [InlineData("program", TokenType.Program)]
        [InlineData("while ", TokenType.While)]
        [InlineData("if ", TokenType.If)]
        [InlineData("else ", TokenType.Else)]
        [InlineData("return ", TokenType.Return)]
        public void SingleToken(string sourceCode, TokenType expectedTokenType)
        {
            StringSourceCodeReader reader = new StringSourceCodeReader(sourceCode);
            Lexer lexer = new Lexer(reader);
            Assert.Equal(lexer.GetNextToken().TokenType, expectedTokenType);
        }
    }
}
