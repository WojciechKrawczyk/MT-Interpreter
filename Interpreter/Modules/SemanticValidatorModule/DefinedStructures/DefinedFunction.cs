using Interpreter.Modules.ParserModule.Structures.Definitions;

namespace Interpreter.Modules.SemanticValidatorModule.DefinedStructures
{
    public class DefinedFunction : FunctionDefinition
    {
        public ScopeContext ScopeContext { get; }

        public DefinedFunction(FunctionDefinition functionDefinition, ScopeContext scopeContext) :
            base(functionDefinition.Name, functionDefinition.Type, functionDefinition.Parameters, functionDefinition.Instructions)
        {
            ScopeContext = scopeContext;
        }
    }
}