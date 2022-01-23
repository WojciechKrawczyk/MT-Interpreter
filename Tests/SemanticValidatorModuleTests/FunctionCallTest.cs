using System.Linq;
using Interpreter.Errors;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class FunctionCallTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { Function(); } }")]
        public void UndefinedFunctionCallTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Call undefined function 'Function'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { Function(2); } def void Function(int a, bool b) { } }")]
        public void ParametersNumberTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Function 'Function' has 2 parameter(s) but is invoked with 1 argument(s)", error);
        }

        [Theory]
        [InlineData("program { def void Main() { Function(2, 1); } def void Function(int a, bool b) { } }")]
        public void ArgumentTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Argument type '{StdTypesNames.Int}' is not assignable to parameter type '{StdTypesNames.Bool}' in function 'Function' call", error);
        }
        
        [Theory]
        [InlineData("program { def void Main() { Function(2, false); } def int Function(int a, bool b) { } }")]
        public void UnusedReturnValueTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var warnings = GetWarningsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(warnings);
            var warning = warnings.ElementAt(0);
            Assert.Equal($"Return value from function 'Function' is not used", warning);
        }
    }
}