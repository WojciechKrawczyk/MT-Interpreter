using Interpreter.Errors;
using Interpreter.Modules.LexerModule;
using Interpreter.Modules.ParserModule;
using Interpreter.Modules.SemanticValidatorModule;
using Interpreter.Modules.SemanticValidatorModule.ValidStructures;
using Interpreter.SourceCodeReader;

namespace Tests.SemanticValidatorModuleTests
{
    public class Tester
    {
        protected ValidProgramInstance ValidateProgramInstance(ErrorsHandler errorsHandler, string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader, errorsHandler);
            var parser = new Parser(lexer, errorsHandler);
            var success = parser.TryToParseProgram(out var program);
            var semanticValidator = new SemanticValidator(errorsHandler);
            return semanticValidator.ValidateProgram(program);
        }
    }
}