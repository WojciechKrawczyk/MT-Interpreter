using System.Collections.Generic;
using Interpreter.Tokens;

namespace Interpreter.ParserModule.Types
{
    public static class TokenTypeToType
    {
        public static readonly Dictionary<TokenType, Type> Map = new()
        {
            {TokenType.Int, Type.Int},
            {TokenType.Bool, Type.Bool},
            {TokenType.Void, Type.Void},
            {TokenType.Identifier, Type.Own}
        };
    }
}