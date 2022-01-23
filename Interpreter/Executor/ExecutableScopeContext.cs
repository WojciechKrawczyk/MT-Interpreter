using System.Collections.Generic;
using Interpreter.Modules.SemanticValidatorModule;
using Interpreter.Modules.SemanticValidatorModule.ValidStructures;

namespace Interpreter.Executor
{
    public class ExecutableScopeContext
    {
        public Dictionary<string, ValidFunction> Functions { get; set; } = new();
        public Dictionary<string, ValidClass> Classes { get; set; } = new();
        public Dictionary<string, ExecutableVariable> Variables { get; set; } = new();

        public ExecutableScopeContext Clone()
        {
            var newExecutableScopeContext = new ExecutableScopeContext
            {
                Functions = Functions,
                Classes = Classes
            };
            foreach (var variable in Variables)
            {
                newExecutableScopeContext.Variables.Add(variable.Key, variable.Value.Clone());
            }

            return newExecutableScopeContext;
        }
    }
}