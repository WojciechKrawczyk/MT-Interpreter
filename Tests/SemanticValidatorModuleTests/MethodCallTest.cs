using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class MethodCallTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { a.Method(); } }")]
        public void UndefinedObjectTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Use of undefined variable 'a'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { int a; a.Method(); } }")]
        public void NotObjectTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Variable 'a' is not an object and does not support '.' operator", error);
        }

        [Theory]
        [InlineData("program { class A { def init() { } def void Method() { } } def void Main() { A a = A(); a.Method1(); } }")]
        public void UndefinedMethodTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Call undefined method 'Method1' on object 'a'", error);
        }

        [Theory]
        [InlineData("program { class A { def init() { } def void Method(int a, bool b) { } } def void Main() { A a = A(); a.Method(1); } }")]
        public void ParametersNumberTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Method 'Method' has 2 parameter(s) but is invoked with 1 argument(s)", error);
        }

        [Theory]
        [InlineData("program { class A { def init() { } def void Method(int a, bool b) { } } def void Main() { A a = A(); a.Method(1, 2); } }")]
        public void ArgumentTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Argument type '{StdTypesNames.Int}' is not assignable to parameter type '{StdTypesNames.Bool}' in method 'Method' call", error);
        }

        [Theory]
        [InlineData("program { class A { def init() { } def int Method() { return 1; } } def void Main() { A a = A(); a.Method(); } }")]
        public void UnusedReturnValueTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var warnings = GetWarningsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(warnings);
            var warning = warnings.ElementAt(0);
            Assert.Equal("Return value from method 'Method' called on object 'a' is not used", warning);
        }

        [Theory]
        [InlineData("program { class A { def init() { } def int Method() { return 1; } } def void Main() { A a; int b = a.Method(); } }")]
        public void NotInitializedObjectTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Variable 'a' is not initialized", error);
        }
    }
}