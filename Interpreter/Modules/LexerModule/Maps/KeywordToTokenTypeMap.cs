using System.Collections.Generic;
using Interpreter.Modules.LexerModule.Tokens;

namespace Interpreter.Modules.LexerModule.Maps
{
    public static class KeywordToTokenTypeMap
    {
        public static readonly Dictionary<string, TokenType> Map = new()
        {
            {"program", TokenType.Program},
            {"class", TokenType.Class},
            {"def", TokenType.Def},
            {"init", TokenType.Init},
            {"int", TokenType.Int},
            {"bool", TokenType.Bool},
            {"false", TokenType.False},
            {"true", TokenType.True},
            {"void", TokenType.Void},
            {"if", TokenType.If},
            {"else", TokenType.Else},
            {"while", TokenType.While},
            {"return", TokenType.Return},
            {"and", TokenType.And},
            {"or", TokenType.Or},
            {"not", TokenType.Not}
        };
    }
}
