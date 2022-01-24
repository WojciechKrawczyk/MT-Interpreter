namespace Interpreter.Modules.SourceCodeReaderModule
{
    public class FileSourceCodeReader : ISourceCodeReader
    {
        private readonly ISourceCodeReader _sourceCodeReader;

        public FileSourceCodeReader(string path)
        {
            var sourceCodeLines = System.IO.File.ReadAllLines(path);
            var code = string.Join('\n', sourceCodeLines);
            _sourceCodeReader = new StringSourceCodeReader(code);
        }

        public char GetNextSymbol() => 
            _sourceCodeReader.GetNextSymbol();

        public bool HasNextSymbol() => 
            _sourceCodeReader.HasNextSymbol();
    }
}