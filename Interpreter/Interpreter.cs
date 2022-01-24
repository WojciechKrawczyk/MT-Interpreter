using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.LexerModule;
using Interpreter.Modules.ParserModule;
using Interpreter.Modules.SourceCodeReaderModule;

namespace Interpreter
{
    public class Interpreter
    {
        public static bool InterpretProgram(ISourceCodeReader sourceCodeReader)
        {
            try
            {
                var errorsHandler = new ErrorsHandler();
                var lexer = new Lexer(sourceCodeReader, errorsHandler);
                var parser = new Parser(lexer, errorsHandler);
                parser.TryToParseProgram(out var program);
                var semanticValidator = new Modules.SemanticValidatorModule.SemanticValidator(errorsHandler);
                var validProgram = semanticValidator.ValidateProgram(program);
                var executor = new Modules.ExecutorModule.Executor();
                executor.ExecuteProgram(validProgram);
                return true;
            }
            catch (StopInterpretationException exception)
            {
                return false;
            }
        }
    }
}