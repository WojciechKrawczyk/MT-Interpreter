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

        public static string Map(Token token)
        {
            var tokenTypeName = token.TokenType.ToString();
            if (_map.TryGetValue(tokenTypeName, out var type)) 
                return type;
            _map.Add(tokenTypeName, tokenTypeName);
            return tokenTypeName;
        }
    }
}