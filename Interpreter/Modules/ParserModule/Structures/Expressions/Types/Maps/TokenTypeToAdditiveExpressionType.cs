using System.Collections.Generic;
using Interpreter.Modules.LexerModule.Tokens;

namespace Interpreter.Modules.ParserModule.Structures.Expressions.Types.Maps
{
    public static class TokenTypeToAdditiveExpressionType
    {
        public static Dictionary<TokenType, AdditiveExpressionType> Map = new()
        {
            {TokenType.Plus, AdditiveExpressionType.Plus},
            {TokenType.Minus, AdditiveExpressionType.Minus}
        };
    }
}