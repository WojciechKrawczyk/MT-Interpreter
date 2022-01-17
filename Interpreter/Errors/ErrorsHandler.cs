using System.Collections.Generic;

namespace Interpreter.Errors
{
    public static class ErrorsHandler
    {
        private static readonly List<string> ErrorMessages = new();
        private static readonly List<string> WarningMessages = new(); 

        public static void HandleError(string errorMessage) => ErrorMessages.Add(errorMessage);
        public static void HandleWarning(string warningMessage) => WarningMessages.Add(warningMessage);
    }
}