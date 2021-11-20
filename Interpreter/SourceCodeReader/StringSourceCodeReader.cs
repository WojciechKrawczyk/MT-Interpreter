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
            _maxPosition = sourceCode.Length - 1;
            _currentPosition = 0;
        }

        public char GetNextSymbol()
        {
            if (_currentPosition > _maxPosition)
                throw new NoSymbolException();

            return _sourceCode[_currentPosition++];
        }

        public bool HasNextSymbol() => _currentPosition <= _maxPosition;
    }
}
