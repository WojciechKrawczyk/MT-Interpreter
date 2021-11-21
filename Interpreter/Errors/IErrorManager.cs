using System.Collections.Generic;

namespace Interpreter.Errors
{
    public interface IErrorManager
    {
        public void AddErrorMessage(string message);
        public IEnumerable<string> GetErrorMessages();
    }
}
