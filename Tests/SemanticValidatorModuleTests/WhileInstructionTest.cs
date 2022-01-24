using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class WhileInstructionTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { int a = 3; while(a) {  } } }")]
        public void UndefinedVariableTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Condition expression has to have '{StdTypesNames.Bool}' type", error);
        }
    }
}