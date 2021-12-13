using System.Linq;
using Interpreter.Lexers;
using Interpreter.ParserModule;
using Interpreter.SourceCodeReader;
using Xunit;

namespace Tests.ParserTests
{
    public class ParserTest
    {
        [Theory]
        [InlineData("program { def void Print(bool a) { } }")]
        public void BaseFunctionDefinitionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);
            var function = program.Functions.ToList().Single().Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Print", function.Name);
            Assert.Single(function.Parameters);
            var parameter = function.Parameters.Single();
            Assert.Equal("a", parameter.Name);
            Assert.Equal("bool", parameter.Type);
        }
        
        [Theory]
        [InlineData("program { class Rectangle { def init(){} } }")]
        public void BaseClassDefinitionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Functions);
            Assert.Single(program.Classes);
            var classDefinition = program.Classes.ToList().Single().Value;
            Assert.Equal("Rectangle", classDefinition.Name);
            Assert.NotNull(classDefinition.Constructor);
        }
    }
}