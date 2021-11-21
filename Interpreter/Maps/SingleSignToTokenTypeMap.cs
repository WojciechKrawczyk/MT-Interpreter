using Interpreter.Tokens;
using System.Collections.Generic;

namespace Interpreter.Maps
{
    public static class SingleSignToTokenTypeMap
    {
        public static Dictionary<char, TokenType> Map = new Dictionary<char, TokenType>
        {
            {'(', TokenType.RoundOpenBracket},
            {')', TokenType.RoundCloseBracker},
            {'{', TokenType.CurlyOpenBracket},
            {'}', TokenType.CurlyCloseBracket},
            {'=', TokenType.Assign},
            {'+', TokenType.Plus},
            {'-', TokenType.Minus},
            {'*', TokenType.Multiplication},
            {'/', TokenType.Division},
            {'%', TokenType.Modulo},
            {'<', TokenType.Less},
            {'>', TokenType.Grater},
            {';', TokenType.Semicolon},
            {',', TokenType.Comma},
            {'.', TokenType.Dot},
        };
    }
}
