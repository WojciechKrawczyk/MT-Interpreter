using Interpreter.Tokens;

namespace Interpreter.Lexers
{
    public interface ILexer
    {
        public Token GetNextToken();
        public Token CurrentToken { get; }
        public void RollbackToken(Token token);
    }
}
