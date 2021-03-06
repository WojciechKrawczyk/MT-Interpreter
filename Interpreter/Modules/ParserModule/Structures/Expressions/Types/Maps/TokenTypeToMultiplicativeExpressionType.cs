using System.Collections.Generic;
using Interpreter.Modules.LexerModule.Tokens;

namespace Interpreter.Modules.ParserModule.Structures.Expressions.Types.Maps
{
    public static class TokenTypeToMultiplicativeExpressionType
    {
        public static Dictionary<TokenType, MultiplicativeExpressionType> Map = new()
        {
            {TokenType.Multiplication, MultiplicativeExpressionType.Multiplication},
            {TokenType.Division, MultiplicativeExpressionType.Division},
            {TokenType.Modulo, MultiplicativeExpressionType.Modulo}
        };
    }
}