using Interpreter.Tokens;

namespace Interpreter.Lexers
{
    public interface ILexer
    {
        public Token GetNextToken();
    }
}
