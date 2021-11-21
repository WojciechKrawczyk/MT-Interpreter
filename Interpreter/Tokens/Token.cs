using Interpreter.Maps;

namespace Interpreter.Tokens
{
    public class Token
    {
        public TokenType TokenType { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }
        public string Lexeme { get; set; }
        public string Value { get; set; }
    }
}
