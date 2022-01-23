using Interpreter.Modules.LexerModule.Tokens;

namespace Interpreter.Modules.LexerModule
{
    public interface ILexer
    {
        public Token GetNextToken();
        public Token CurrentToken { get; }
        public void RollbackToken(Token token);
    }
}
