using System.Collections.Generic;

namespace Interpreter.Errors
{
    class ErrorManager : IErrorManager
    {
        private List<string> _errorMessages;

        public ErrorManager()
        {
            _errorMessages = new List<string>();
        }

        public void AddErrorMessage(string message) => _errorMessages.Add(message);
        public IEnumerable<string> GetErrorMessages() => _errorMessages;
    }
}
