using System.Collections.Generic;
using System.Linq;

namespace Interpreter.SemanticValidator
{
    public class ScopeContext
    {
        public List<DefinedVariable> DefinedVariables { get; } = new();

        public bool TryAddVariable(DefinedVariable variable)
        {
            if (DefinedVariables.Exists(x => x.Name == variable.Name))
            {
                return false;
            }
            DefinedVariables.Add(variable);
            return true;
        }

        public void AddVariable(DefinedVariable variable) => DefinedVariables.Add(variable);

        public bool TryGetVariable(string name, out DefinedVariable variable)
        {
            variable = null;
            if (!DefinedVariables.Exists(x => x.Name == name))
            {
                return false;
            }
            variable = DefinedVariables.First(x => x.Name == name);
            return true;
        }

        public bool HasVariable(string name) => DefinedVariables.Exists(x => x.Name == name);
        public bool IsVariableInitialized(string name) => DefinedVariables.First(x => x.Name == name).IsInitialized;
        public void SetVariableInitialized(string name) => DefinedVariables.First(x => x.Name == name).InitializeVariable();
    }
}