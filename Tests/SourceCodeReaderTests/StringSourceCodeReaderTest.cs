using Interpreter.SourceCodeReader;
using System.Text;
using Xunit;

namespace Tests.SourceCodeReaderTests
{
    public class StringSourceCodeReaderTest
    {
        [Theory]
        [InlineData("qwertyiopasdfghjklzxcvbm")]
        [InlineData("QWERTYUIOPASDFGHJKLZXCVBNM")]
        public void AlphabetCharactersTest(string sourceCode)
        {
            Assert.Equal(GenerateResult(sourceCode), sourceCode);
        }

        [Theory]
        [InlineData("0123456789")]
        public void NumericCharactersTest(string sourceCode)
        {
            Assert.Equal(GenerateResult(sourceCode), sourceCode);
        }

        [Theory]
        [InlineData("+-*/%")]
        [InlineData("=!><")]
        [InlineData("(){}")]
        [InlineData("#")]
        [InlineData(";")]
        [InlineData(".")]
        [InlineData(",")]
        [InlineData(" ")]
        [InlineData("@^[]:?'~_$&`|")]
        public void SpecialCharactersTest(string sourceCode)
        {
            Assert.Equal(GenerateResult(sourceCode), sourceCode);
        }

        [Theory]
        [InlineData("\n\\\"")]
        public void EscapedCharactersTest(string sourceCode)
        {
            Assert.Equal(GenerateResult(sourceCode), sourceCode);
        }

        private static string GenerateResult(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var result = new StringBuilder();
            while (reader.HasNextSymbol())
            {
                result.Append(reader.GetNextSymbol());
            }
            return result.ToString();
        }
    }
}
