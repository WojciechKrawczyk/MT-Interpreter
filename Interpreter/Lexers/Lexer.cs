using Interpreter.Errors;
using Interpreter.Maps;
using Interpreter.SourceCodeReader;
using Interpreter.Tokens;
using System.Text;

namespace Interpreter.Lexers
{
    public class Lexer : ILexer
    {
        private readonly ISourceCodeReader _sourceCodeReader;
        private int _line = 1;
        private int _position = 0;
        private char _bufferedSymbol;
        private bool _hasBufferedSymbol = false;

        public Lexer(ISourceCodeReader sourceCodeReader)
        {
            _sourceCodeReader = sourceCodeReader;
        }

        public Token GetNextToken()
        {
            if (!_sourceCodeReader.HasNextSymbol())
                return GenerateToken(TokenType.EndOfFile, "Eof");

            var buffor = new StringBuilder();
            while(true)
            {
                var symbol = GetNextSymbol();
                if (symbol == ' ')
                {
                    _position++;
                    continue;
                }
                else if (symbol == '\n')
                {
                    _line += 1;
                    _position = 0;
                    continue;
                }
                else if (symbol == '#')
                {
                    symbol = GetNextSymbol();
                    while (symbol != '\n')
                    {
                        symbol = GetNextSymbol();
                    }
                    continue;
                }
                else if (char.IsLetter(symbol))
                {
                    buffor.Append(symbol);
                    symbol = GetNextSymbol();
                    while (char.IsLetterOrDigit(symbol))
                    {
                        buffor.Append(symbol);
                        symbol = GetNextSymbol();
                    }
                    SetBufferedSymbol(symbol);
                    var lexeme = buffor.ToString(); //TODO error - too long
                    if (KeywordToTokenTypeMap.Map.ContainsKey(lexeme))
                        return GenerateToken(KeywordToTokenTypeMap.Map[lexeme], lexeme);
                    else
                        return GenerateToken(TokenType.Identifier, lexeme);
                }
                else if (char.IsDigit(symbol))
                {
                    buffor.Append(symbol);
                    symbol = GetNextSymbol();
                    while (char.IsDigit(symbol))
                    {
                        buffor.Append(symbol);
                        symbol = GetNextSymbol();
                    }
                    SetBufferedSymbol(symbol);
                    var number = buffor.ToString(); //TODO error - check range
                    return GenerateToken(TokenType.IntLiteral, number, number);
                }
                else if (symbol == '"')
                {
                    symbol = GetNextSymbol();
                    while(symbol != '"')
                    {
                        if(symbol == '\\')
                        {
                            var nextSymbol = GetNextSymbol();
                            if (nextSymbol == '\\')
                                buffor.Append('\\');
                            else if (nextSymbol == 'n')
                                buffor.Append('\n');
                            else if (nextSymbol == '"')
                                buffor.Append('"');
                            else if (nextSymbol == 't')
                                buffor.Append('\t');
                            else
                                buffor.Append(nextSymbol);  //TODO: error - not valid char in string
                            symbol = GetNextSymbol();
                            continue;
                        }
                        symbol = GetNextSymbol();
                    }
                    var stringLiteral = buffor.ToString();
                    return GenerateToken(TokenType.StringLiteral, stringLiteral);
                }
                else
                {
                    switch (symbol)
                    {
                        case '=':
                            return CheckMatch(symbol, '=', TokenType.Assign, TokenType.Equal);
                        case '!':
                            return CheckMatch(symbol, '=', TokenType.Invalid, TokenType.NotEqual);
                        case '<':
                            return CheckMatch(symbol, '=', TokenType.Less, TokenType.LessOrEqual);
                        case '>':
                            return CheckMatch(symbol, '=', TokenType.Grater, TokenType.GraterOrEqual);
                        default:
                            if (SingleSignToTokenTypeMap.Map.ContainsKey(symbol))
                                return GenerateToken(SingleSignToTokenTypeMap.Map[symbol], symbol.ToString());
                            return GenerateToken(TokenType.Invalid, symbol.ToString());
                    }
                }
            }
        }

        private void SetBufferedSymbol(char symbol)
        {
            _bufferedSymbol = symbol;
            _hasBufferedSymbol = true;
        }

        private char GetNextSymbol()
        {
            if (_hasBufferedSymbol)
            {
                _hasBufferedSymbol = false;
                return _bufferedSymbol;
            }
            _position++;
            return _sourceCodeReader.GetNextSymbol();
        }

        private Token GenerateToken(TokenType tokenType, string lexeme, string value = null)
            => new Token
            {
                TokenType = tokenType,
                Lexeme = lexeme,
                Line = _line,
                Position = _position,
                Value = value
            };

        private Token CheckMatch(char currentSymbol, char symbolToMatch, TokenType basicTokenType, TokenType tokenTypeToMatch)
        {
            var nextSymbol = GetNextSymbol();
            if (nextSymbol == symbolToMatch)
                return GenerateToken(tokenTypeToMatch, $"{currentSymbol}{symbolToMatch}");
            SetBufferedSymbol(nextSymbol);
            return GenerateToken(basicTokenType, currentSymbol.ToString());
        }
    }
}