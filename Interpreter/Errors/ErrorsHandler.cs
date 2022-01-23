using System;
using System.Collections.Generic;

namespace Interpreter.Errors
{
    public class ErrorsHandler
    {
        public List<string> Errors => _errorMessages;


        private readonly List<string> _errorMessages = new();
        private readonly List<string> _warningMessages = new(); 

        public void HandleError(string errorMessage) => _errorMessages.Add(errorMessage);
        public void HandleWarning(string warningMessage) => _warningMessages.Add(warningMessage);

        public void HandleFatalError(string fatalErrorMessage)
        {
            PrintWarningAndErrors();
            Console.WriteLine($"FATAL ERROR: {fatalErrorMessage}");
            throw new StopInterpretationException();
        }

        public void StopInterpretation()
        {
            PrintWarningAndErrors();
            throw new StopInterpretationException();
        }

        private void PrintWarningAndErrors()
        {
            foreach (var warningMessage in _warningMessages)
            {
                Console.WriteLine($"WARNING: {warningMessage}");
            }

            foreach (var errorMessage in _errorMessages)
            {
                Console.WriteLine($"ERROR: {errorMessage}");
            }
        }
    }
}