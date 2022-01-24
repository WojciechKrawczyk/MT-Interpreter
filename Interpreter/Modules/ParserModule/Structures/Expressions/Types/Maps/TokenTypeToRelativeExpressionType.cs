using System.Collections.Generic;
using Interpreter.Modules.LexerModule.Tokens;

namespace Interpreter.Modules.ParserModule.Structures.Expressions.Types.Maps
{
    public static class TokenTypeToRelativeExpressionType
    {
        public static Dictionary<TokenType, RelativeExpressionType> Map = new()
        {
            {TokenType.Less, RelativeExpressionType.Less},
            {TokenType.LessOrEqual, RelativeExpressionType.LessOrEqual},
            {TokenType.Grater, RelativeExpressionType.Grater},
            {TokenType.GraterOrEqual, RelativeExpressionType.GraterOrEqual},
            {TokenType.Equal, RelativeExpressionType.Equal},
            {TokenType.NotEqual, RelativeExpressionType.NotEqual}
        };
    }
}