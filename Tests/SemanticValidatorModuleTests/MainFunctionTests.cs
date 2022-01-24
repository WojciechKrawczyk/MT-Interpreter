using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class MainFunctionTests : Tester
    {
        [Theory]
        [InlineData("program { }")]
        public void NoMainFunctionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"No '{StdNames.MainFunctionName}' function is defined", error);
        }

        [Theory]
        [InlineData("program { def int Main() { } }")]
        public void MainFunctionTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Function '{StdNames.MainFunctionName} have to have '{StdTypesNames.Void}' type", error);
        }

        [Theory]
        [InlineData("program { def void Main(int a) { } }")]
        public void MainFunctionParametersTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Function '{StdNames.MainFunctionName} can not have parameters", error);
        }
        
        [Theory]
        [InlineData("program { def void Main() { } }")]
        public void CorrectMainFunctionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }
    }
}