using System.Linq;
using Interpreter.Errors;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class FunctionDefinitionTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { } def X Function() { } }")]
        public void UnknownReturnTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Unknown type 'X' in function 'Function' definition", error);
        }

        [Theory]
        [InlineData("program { def void Main() { } def void A() { } class A { def init() { } } }")]
        public void ConflictWithClassTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Function 'A' conflicts with already defined class 'A'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { } def void Function() { } def void Function() { } }")]
        public void RedefinitionFunctionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redefinition of function 'Function'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { } def void PrintInt() { } }")]
        public void RedefinitionStandardLibFunctionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redefinition of standard lib function 'PrintInt'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { } def void Function(X x) { } }")]
        public void UnknownParameterTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Unknown type 'X' for 'x' parameter in function 'Function' definition", error);
        }

        [Theory]
        [InlineData("program { class A { def init() { } } def void Main() { } def void Function(A a, int a) { } }")]
        public void RedefinitionParameterTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redefinition of parameter 'a' in function 'Function' definition", error);
        }
    }
}