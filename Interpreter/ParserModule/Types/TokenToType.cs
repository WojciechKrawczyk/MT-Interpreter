using System.Collections.Generic;
using Interpreter.Tokens;

namespace Interpreter.ParserModule.Types
{
    public static class TokenToType
    {
        private static readonly Dictionary<string, string> _map = new()
        {
            {"Int", "int"},
            {"Bool", "bool"},
            {"Void", "void"}
        };

        public static string Map(Token token) => 
            _map.TryGetValue(token.TokenType.ToString(), out var type) 
                ? type 
                : token.Lexeme;
    }
}