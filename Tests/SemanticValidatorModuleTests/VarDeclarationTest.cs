using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.StdResources;
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
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Declaration variable 'a' of unknown type 'A'", error);
        }
        
        [Theory]
        [InlineData("program { def void Main() { } def int Function() { int a; bool a; } }")]
        public void RedeclarationTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redeclaration of variable 'a'", error);
        }
        
        [Theory]
        [InlineData("program { def void Main() { } def int Function() { int a = false; } }")]
        public void TypesConflictTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Unable assign expression with type '{StdTypesNames.Bool}' to variable 'a' with type '{StdTypesNames.Int}'", error);
        }
    }
}