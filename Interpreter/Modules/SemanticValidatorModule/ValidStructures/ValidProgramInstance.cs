using System.Collections.Generic;

namespace Interpreter.Modules.SemanticValidatorModule.ValidStructures
{
    public class ValidProgramInstance
    {
        public Dictionary<string, ValidFunction> Functions { get; init; }
        public Dictionary<string, ValidClass> Classes { get; init; }
    }
}