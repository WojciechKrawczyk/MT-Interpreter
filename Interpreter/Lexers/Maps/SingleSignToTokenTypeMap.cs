using System.Collections.Generic;
using Interpreter.Tokens;

namespace Interpreter.Lexers.Maps
{
    public static class SingleSignToTokenTypeMap
    {
        public static readonly Dictionary<char, TokenType> Map = new()
        {
            {'(', TokenType.RoundOpenBracket},
            {')', TokenType.RoundCloseBracket},
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
