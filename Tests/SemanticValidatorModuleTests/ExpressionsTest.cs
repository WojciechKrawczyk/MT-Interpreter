using System.Linq;
using Interpreter.Errors;
using Interpreter.Modules.StdResources;
using Xunit;

namespace Tests.SemanticValidatorModuleTests
{
    public class ExpressionsTest : Tester
    {
        [Theory]
        [InlineData("program { def void Main() { bool t = 3 or false; } }")]
        public void OrExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not apply operator 'or' to operands of type '{StdTypesNames.Int}' and '{StdTypesNames.Bool}'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { bool t = true or false; } }")]
        public void ValidOrExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("program { def void Main() { bool t = 3 and false; } }")]
        public void AndExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not apply operator 'and' to operands of type '{StdTypesNames.Int}' and '{StdTypesNames.Bool}'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { bool t = 3 < false; } }")]
        public void RelativeExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not apply operator '<' to operands of type '{StdTypesNames.Int}' and '{StdTypesNames.Bool}'", error);
        }

        [Theory]
        [InlineData("program { class A { def init() { } int three = 3; } def void Main() { A a = A(); bool t = 3 < a.three; } }")]
        public void ValidRelativeExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("program { def void Main() { int t = 3 + false; } }")]
        public void AdditiveExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not apply operator '+' to operands of type '{StdTypesNames.Int}' and '{StdTypesNames.Bool}'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { int t = 3 + 5; } }")]
        public void ValidAdditiveExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("program { def void Main() { int t = 3 * false; } }")]
        public void MultiplicativeExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not apply operator '*' to operands of type '{StdTypesNames.Int}' and '{StdTypesNames.Bool}'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { int t = 3 * 5; } }")]
        public void ValidMultiplicativeExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("program { def void Main() { bool t = not 3; } }")]
        public void NotExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not apply operator 'not' to operand of type '{StdTypesNames.Int}'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { bool t = not false; } }")]
        public void ValidNotExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }
        
        [Theory]
        [InlineData("program { def void Main() { bool t = a; } }")]
        public void VariableExpressionUndefinedVariableTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Equal(2, errors.Count);
            var error1 = errors.ElementAt(0);
            Assert.Equal($"Usage of undefined variable 'a'", error1);
            var error2 = errors.ElementAt(1);
            Assert.Equal($"Unable assign expression with type '' to variable 't' with type '{StdTypesNames.Bool}'", error2);
        }

        [Theory]
        [InlineData("program { def void Main() { bool a; bool t = a; } }")]
        public void VariableExpressionUninitializedVariableTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal($"Usage of uninitialized variable 'a'", error);
        }

        [Theory]
        [InlineData("program { def void Main() { bool a = false; bool b = a; } }")]
        public void ValidVariableExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("program { def void Main() { bool b = a.A; } }")]
        public void PropertyExpressionTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Equal(2, errors.Count);
            var error1 = errors.ElementAt(0);
            Assert.Equal($"Use of undefined variable 'a'", error1);
            var error2 = errors.ElementAt(1);
            Assert.Equal($"Unable assign expression with type '' to variable 'b' with type '{StdTypesNames.Bool}'", error2);
        }

        [Theory]
        [InlineData("program { def void Main() { int a = 3; bool b = a.A; } }")]
        public void PropertyExpressionNoObjectTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Equal(2, errors.Count);
            var error1 = errors.ElementAt(0);
            Assert.Equal("Variable 'a' is not an object and does not support '.' operator", error1);
            var error2 = errors.ElementAt(1);
            Assert.Equal($"Unable assign expression with type '' to variable 'b' with type '{StdTypesNames.Bool}'", error2);
        }

        [Theory]
        [InlineData("program {  class A { def init() { } int three = 3; } def void Main() { A a; int b = a.three; } }")]
        public void PropertyExpressionUninitializedObjectTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Single(errors);
            var error = errors.ElementAt(0);
            Assert.Equal("Variable 'a' is not initialized", error);
        }

        [Theory]
        [InlineData("program {  class A { def init() { } int three = 3; } def void Main() { A a = A(); int b = a.two; } }")]
        public void PropertyExpressionUndefinedPropertyTest(string sourceCode)
        {
            var errorsHandler = new ErrorsHandler();
            var errors = GetErrorsFromProgramInstance(errorsHandler, sourceCode);
            Assert.Equal(2, errors.Count);
            var error = errors.ElementAt(0);
            Assert.Equal($"Can not resolve property 'two' for variable 'a' with type 'A", error);
            var error2 = errors.ElementAt(1);
            Assert.Equal($"Unable assign expression with type '' to variable 'b' with type '{StdTypesNames.Int}'", error2);
        }
    }
}