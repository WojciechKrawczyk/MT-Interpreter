using System;
using System.Collections.Generic;
using Interpreter.Errors;
using Interpreter.Modules.LexerModule;
using Interpreter.Modules.ParserModule;
using Interpreter.Modules.SemanticValidatorModule;
using Interpreter.SourceCodeReader;

namespace Tests.SemanticValidatorModuleTests
{
    public class Tester
    {
        protected List<string> GetErrorsFromProgramInstance(ErrorsHandler errorsHandler, string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader, errorsHandler);
            var parser = new Parser(lexer, errorsHandler);
            var success = parser.TryToParseProgram(out var program);
            var semanticValidator = new SemanticValidator(errorsHandler);
            try
            {
                semanticValidator.ValidateProgram(program);
            }
            catch (Exception)
            {
                // ignored
            }
            return errorsHandler.Errors;
        }
        
        protected List<string> GetWarningsFromProgramInstance(ErrorsHandler errorsHandler, string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader, errorsHandler);
            var parser = new Parser(lexer, errorsHandler);
            var success = parser.TryToParseProgram(out var program);
            var semanticValidator = new SemanticValidator(errorsHandler);
            try
            {
                semanticValidator.ValidateProgram(program);
            }
            catch (Exception)
            {
                // ignored
            }
            return errorsHandler.Warnings;
        }
    }
}