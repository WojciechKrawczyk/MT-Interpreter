using System;
using System.Text;
using Interpreter.Errors;
using Interpreter.Extensions;
using Interpreter.Modules.LexerModule.Maps;
using Interpreter.Modules.LexerModule.Tokens;
using Interpreter.SourceCodeReader;

namespace Interpreter.Modules.LexerModule
{
    public class Lexer : ILexer
    {
        public Token CurrentToken { get; private set; }

        private readonly ISourceCodeReader _sourceCodeReader;
        private readonly ErrorsHandler _errorsHandler;

        private Token _nextToken;
        private int _line = 1;
        private int _position = 1;
        private char _bufferedSymbol;
        private bool _hasBufferedSymbol;
        private char _symbol;
        private const int LengthThreshold = 100;

        public Lexer(ISourceCodeReader sourceCodeReader, ErrorsHandler errorsHandler)
        {
            _sourceCodeReader = sourceCodeReader;
            _errorsHandler = errorsHandler;
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
                var i = 0;
                while (char.IsWhiteSpace(_symbol))
                {
                    CheckThreshold(i);
                    _position++;
                    _symbol = GetNextSymbol();
                    i++;
                }
            }

            void HandleComment()
            {
                var i = 0;
                while (!_symbol.IsEndOfLine())
                {
                    CheckThreshold(i);
                    _symbol = GetNextSymbol();
                    i++;
                }
            }

            void HandleEndOfLine()
            {
                _line += 1;
                _position = 1;
                var previous = _symbol;
                _symbol = GetNextSymbol();
                if (previous == '\r' && _symbol == '\n')
                    _symbol = GetNextSymbol();
            }
        }
        
        private Token HandleWord()
        {
            var buffor = new StringBuilder();
            buffor.Append(_symbol);
            _symbol = GetNextSymbol();
            var i = 0;
            while (char.IsLetterOrDigit(_symbol))
            {
                CheckThreshold(i);
                buffor.Append(_symbol);
                _symbol = GetNextSymbol();
                i++;
            }

            SetBufferedSymbol(_symbol);
            var lexeme = buffor.ToString();
            return GenerateToken(KeywordToTokenTypeMap.Map.ContainsKey(lexeme)
                ? KeywordToTokenTypeMap.Map[lexeme]
                : TokenType.Identifier, lexeme);
        }

        private Token HandleNumber()
        {
            var buffor = new StringBuilder();
            buffor.Append(_symbol);
            _symbol = GetNextSymbol();
            var i = 0;
            while (char.IsDigit(_symbol))
            {
                CheckThreshold(i);
                buffor.Append(_symbol);
                _symbol = GetNextSymbol();
                i++;
            }

            SetBufferedSymbol(_symbol);
            var number = buffor.ToString();
            CheckNumber(number);

            return GenerateToken(TokenType.IntLiteral, number, number);
        }

        private Token HandleString()
        {
            var buffor = new StringBuilder();
            _symbol = GetNextSymbol();
            var i = 0;
            while (_symbol != '"')
            {
                CheckThreshold(i);
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
                        _errorsHandler.HandleFatalError($"Invalid char in string declaration [Line: {_line}, Position: {_position}]");
                    _symbol = GetNextSymbol();
                    continue;
                }

                _symbol = GetNextSymbol();
                i++;
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

            try
            {
                _position++;
                return _sourceCodeReader.GetNextSymbol();
            }
            catch (Exception)
            {
                _errorsHandler.HandleFatalError("Unexpected end of source file");
                return '_';
            }
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

        private void CheckThreshold(int i)
        {
            if (i > LengthThreshold)
            {
                _errorsHandler.HandleFatalError($"Reached the length threshold of persistent signs sequence [Line: {_line}, Position: {_position}]");
            }
        }

        private void CheckNumber(string number)
        {
            try
            {
                var _ = int.Parse(number);
            }
            catch (Exception)
            {
                _errorsHandler.HandleFatalError($"Integer overflow in number declaration [Line: {_line}, Position: {_position}]");
            }
        }
    }
}