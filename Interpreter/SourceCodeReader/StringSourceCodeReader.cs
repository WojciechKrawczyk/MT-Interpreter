namespace Interpreter.SourceCodeReader
{
    public class StringSourceCodeReader : ISourceCodeReader
    {
        private readonly string _sourceCode;
        private readonly int _maxPosition;
        private int _currentPosition;
        public StringSourceCodeReader(string sourceCode)
        {
            _sourceCode = sourceCode;
            _maxPosition = sourceCode.Length;
            _currentPosition = 0;
        }

        public char GetNextSymbol()
        {
            if (_currentPosition > _maxPosition)
                throw new NoSymbolException();

            return _currentPosition < _maxPosition 
                ? _sourceCode[_currentPosition++] 
                : (char)3;
        }

        public bool HasNextSymbol() => _currentPosition < _maxPosition;
    }
}
