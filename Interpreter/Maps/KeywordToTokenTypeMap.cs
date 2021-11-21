using Interpreter.Tokens;
using System.Collections.Generic;

namespace Interpreter.Maps
{
    public static class KeywordToTokenTypeMap
    {
        public static Dictionary<string, TokenType> Map = new Dictionary<string, TokenType>
        {
            {"program", TokenType.Program},
            {"class", TokenType.Class},
            {"def", TokenType.Def},
            {"int", TokenType.Int},
            {"bool", TokenType.Bool},
            {"false", TokenType.False},
            {"true", TokenType.True},
            {"void", TokenType.Void},
            {"if", TokenType.If},
            {"else", TokenType.Else},
            {"while", TokenType.While},
            {"return", TokenType.Return}
        };
    }
}
