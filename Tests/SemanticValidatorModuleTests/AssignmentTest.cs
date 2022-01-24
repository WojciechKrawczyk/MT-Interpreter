using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class AssignmentTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { } def int Function() { a = 3; } }")]
        public void UndefinedVariableTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Assignment to undefined variable 'a'", error);
        }
        
        [Theory]
        [InlineData("program { def void Main() { } def int Function() { int a; a = false; } }")]
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