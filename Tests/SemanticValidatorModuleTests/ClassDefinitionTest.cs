using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class ClassDefinitionTest : Tester
    {
        [Theory]
        [InlineData("program { class A { def init() { } } class A { def init() { } } def void Main() { } }")]
        public void RedefinitionClassTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redefinition of class 'A'", error);
        }

        [Theory] [InlineData("program { class Main { def init() { } } def void Main() { } }")]
        public void MainClassTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Class can not be defined with name '{StdNames.MainFunctionName}'", error);
        }

        [Theory] [InlineData("program { class C { def init() { } A a; } def void Main() { } }")]
        public void UnknownPropertyTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Declaration property 'a' of unknown type 'A' in class 'C' definition", error);
        }

        [Theory] [InlineData("program { class C { def init() { } int A; int A; } def void Main() { } }")]
        public void RedefinitionPropertyTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Redefinition of property 'A' in class 'C' definition", error);
        }

        [Theory] [InlineData("program { class C { def init() { } int A = false; } def void Main() { } }")]
        public void WrongExpressionTypeForPropertyTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Unable assign expression with type '{StdTypesNames.Bool}' to property 'A' with type '{StdTypesNames.Int}' in class 'C' definition", error);
        }

        [Theory] [InlineData("program { class C { def init() { } def A Method() { } } def void Main() { } }")]
        public void UnknownMethodTypeTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Unknown type 'A' in method 'Method' definition in 'C' class", error);
        }

        [Theory] [InlineData("program { class C { def init() { } def int Method() { } def int Method() { } } def void Main() { } }")]
        public void RedefinitionMethodTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Redefinition of method 'Method' in 'C' class", error);
        }

        [Theory] [InlineData("program { class C { def init() { } def int Method(A a) { } } def void Main() { } }")]
        public void UnknownTypeInMethodParameterTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Unknown type 'A' for 'a' parameter in method 'Method' definition in 'C' class", error);
        }

        [Theory] [InlineData("program { class C { def init() { } int a; def int Method(bool a) { } } def void Main() { } }")]
        public void ConflictParameterWithPropertyTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Parameter 'a' conflicts with property in method 'Method' definition in 'C' class", error);
        }

        [Theory] [InlineData("program { class C { def init() { } def int Method(bool a, int a) { } } def void Main() { } }")]
        public void MethodParameterRedefinitionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Redefinition of parameter 'a' in method 'Method' definition", error);
        }

        [Theory] [InlineData("program { class C { def init(A a) { } } def void Main() { } }")]
        public void UnknownTypeInConstructorParameterTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Unknown type 'A' for 'a' parameter in constructor definition in 'C' class", error);
        }

        [Theory] [InlineData("program { class C { def init(bool a) { } int a; } def void Main() { } }")]
        public void ConflictConstructorParameterWithPropertyTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Parameter 'a' conflicts with property in constructor definition in 'C' class", error);
        }

        [Theory] [InlineData("program { class C { def init(int a, bool a) { } } def void Main() { } }")]
        public void ConstructorParameterRedefinitionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Redefinition of parameter 'a' in constructor definition in 'C' class", error);
        }
    }
}