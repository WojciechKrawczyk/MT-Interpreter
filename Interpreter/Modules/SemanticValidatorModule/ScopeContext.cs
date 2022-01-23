using System.Collections.Generic;
using Interpreter.Modules.SemanticValidatorModule.DefinedStructures;

namespace Interpreter.Modules.SemanticValidatorModule
{
    public class ScopeContext
    {
        public Dictionary<string, DefinedFunction> DefinedFunctions { get; set; } = new();
        public Dictionary<string, DefinedVariable> DefinedVariables { get; } = new();

        public ScopeContext(){}

        public ScopeContext(ScopeContext scopeContext)
        {
            foreach (var variable in scopeContext.DefinedVariables.Values)
            {
                DefinedVariables.Add(variable.Name, new DefinedVariable(variable.Type, variable.Name, variable.IsInitialized));
            }
            DefinedFunctions = scopeContext.DefinedFunctions;
        }

        public ScopeContext(Dictionary<string, DefinedFunction> definedFunctions, Dictionary<string, DefinedVariable> definedVariables)
        {
            DefinedFunctions = definedFunctions;
            foreach (var variable in definedVariables.Values)
            {
                DefinedVariables.Add(variable.Name, new DefinedVariable(variable.Type, variable.Name, variable.IsInitialized));
            }
        }
    }
}