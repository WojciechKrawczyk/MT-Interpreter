using System.Collections.Generic;
using Interpreter.Modules.LexerModule.Tokens;

namespace Interpreter.Modules.LexerModule.Maps
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
