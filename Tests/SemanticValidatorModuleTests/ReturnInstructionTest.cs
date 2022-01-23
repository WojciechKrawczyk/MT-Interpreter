using System.Linq;
using Interpreter.Errors;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class ReturnInstructionTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { int a = 3; }  def void Function() { int a = 3; return a; } }")]
        public void UndefinedVariableTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not convert expression type '{StdTypesNames.Int}' to return function type '{StdTypesNames.Void}'", error);
        }
    }
}