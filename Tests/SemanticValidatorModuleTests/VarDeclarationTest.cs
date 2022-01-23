using System.Linq;
using Interpreter.Errors;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class VarDeclarationTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { } def int Function() { A a; } }")]
        public void UnknownTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var validProgram = ValidateProgramInstance(errorsHandler, sourceCode);
            var errors = errorsHandler.Errors;
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Declaration variable 'a' of unknown type 'A'", error);
        }
        
        [Theory]
        [InlineData("program { def void Main() { } def int Function() { int a; bool a; } }")]
        public void RedeclarationTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var validProgram = ValidateProgramInstance(errorsHandler, sourceCode);
            var errors = errorsHandler.Errors;
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redeclaration of variable 'a'", error);
        }
    }
}