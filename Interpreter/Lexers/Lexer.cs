using System.Text;
using Interpreter.Extensions;
using Interpreter.Lexers.Maps;
using Interpreter.SourceCodeReader;
using Interpreter.Tokens;

namespace Interpreter.Lexers
{
    public class Lexer : ILexer
    {
        public Token CurrentToken { get; private set; }
        private Token _nextToken = null;
        private readonly ISourceCodeReader _sourceCodeReader;
        private int _line = 1;
        private int _position = 0;
        private char _bufferedSymbol;
        private bool _hasBufferedSymbol = false;
        private char _symbol;

        public Lexer(ISourceCodeReader sourceCodeReader)
        {
            _sourceCodeReader = sourceCodeReader;
        }

        public Token GetNextToken()
        {
            if (_nextToken != null)
            {
                var tmp = _nextToken;
                _nextToken = null;
                return tmp;
            }
            CurrentToken = BuildNextToken();
            return CurrentToken;
        }

        public void RollbackToken(Token token)
        {
            _nextToken = token;
        }

        private Token BuildNextToken()
        {
            if (!_sourceCodeReader.HasNextSymbol())
                return GenerateToken(TokenType.EndOfFile, "Eof");

            IgnoreNonSignificantSymbols();

            if (char.IsLetter(_symbol))
                return HandleWord();

            if (char.IsDigit(_symbol))
                return HandleNumber();

            if (_symbol == '"')
                return HandleString();

            return HandleSpecialSymbol();
        }

        private Token HandleWord()
        {
            var buffor = new StringBuilder();
            buffor.Append(_symbol);
            _symbol = GetNextSymbol();
            while (char.IsLetterOrDigit(_symbol))
            {
                buffor.Append(_symbol);
                _symbol = GetNextSymbol();
            }

            SetBufferedSymbol(_symbol);
            var lexeme = buffor.ToString(); //TODO error - too long
            return GenerateToken(KeywordToTokenTypeMap.Map.ContainsKey(lexeme)
                ? KeywordToTokenTypeMap.Map[lexeme]
                : TokenType.Identifier, lexeme);
        }

        private void IgnoreNonSignificantSymbols()
        {
            _symbol = GetNextSymbol();
            while (char.IsWhiteSpace(_symbol) || _symbol.IsEndOfLine() || _symbol == '#')
            {
                if (char.IsWhiteSpace(_symbol))
                    HandleWhiteSpaces();
                else if (_symbol == '#')
                    HandleComment();
                else
                    HandleEndOfLine();
            }

            void HandleWhiteSpaces()
            {
                while (char.IsWhiteSpace(_symbol))
                {
                    _position++;
                    _symbol = GetNextSymbol();
                }
            }

            void HandleComment()
            {
                while (!_symbol.IsEndOfLine()) 
                    _symbol = GetNextSymbol();
            }

            void HandleEndOfLine()
            {
                _line += 1;
                _position = 0;
                var previous = _symbol;
                _symbol = GetNextSymbol();
                if (previous == '\r' && _symbol == '\n')
                    _symbol = GetNextSymbol();
            }
        }

        private Token HandleNumber()
        {
            var buffor = new StringBuilder();
            buffor.Append(_symbol);
            _symbol = GetNextSymbol();
            while (char.IsDigit(_symbol))
            {
                buffor.Append(_symbol);
                _symbol = GetNextSymbol();
            }

            SetBufferedSymbol(_symbol);
            var number = buffor.ToString(); //TODO error - check range
            return GenerateToken(TokenType.IntLiteral, number, number);
        }

        private Token HandleString()
        {
            var buffor = new StringBuilder();
            _symbol = GetNextSymbol();
            while (_symbol != '"')
            {
                if (_symbol == '\\')
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
                        buffor.Append(nextSymbol); //TODO: error - not valid char in string
                    _symbol = GetNextSymbol();
                    continue;
                }

                _symbol = GetNextSymbol();
            }

            var stringLiteral = buffor.ToString();
            return GenerateToken(TokenType.StringLiteral, stringLiteral);
        }

        private Token HandleSpecialSymbol()
        {
            return _symbol switch
            {
                '=' => CheckMatch(_symbol, '=', TokenType.Assign, TokenType.Equal),
                '!' => CheckMatch(_symbol, '=', TokenType.Invalid, TokenType.NotEqual),
                '<' => CheckMatch(_symbol, '=', TokenType.Less, TokenType.LessOrEqual),
                '>' => CheckMatch(_symbol, '=', TokenType.Grater, TokenType.GraterOrEqual),
                _ => GenerateToken(
                    SingleSignToTokenTypeMap.Map.ContainsKey(_symbol)
                        ? SingleSignToTokenTypeMap.Map[_symbol]
                        : TokenType.Invalid, _symbol.ToString())
            };

            Token CheckMatch(char currentSymbol, char symbolToMatch, TokenType basicTokenType, TokenType tokenTypeToMatch)
            {
                var nextSymbol = GetNextSymbol();
                if (nextSymbol == symbolToMatch)
                    return GenerateToken(tokenTypeToMatch, $"{currentSymbol}{symbolToMatch}");
                SetBufferedSymbol(nextSymbol);
                return GenerateToken(basicTokenType, currentSymbol.ToString());
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

        private Token GenerateToken(TokenType tokenType, string lexeme, string value = null) => 
            new()
            {
                TokenType = tokenType,
                Lexeme = lexeme,
                Line = _line,
                Position = _position,
                Value = value
            };
    }
}