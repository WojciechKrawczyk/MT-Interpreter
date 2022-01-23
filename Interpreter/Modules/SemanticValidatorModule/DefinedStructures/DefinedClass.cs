using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;

namespace Interpreter.Modules.SemanticValidatorModule.DefinedStructures
{
    public class DefinedClass : ClassDefinition
    {
        public ScopeContext ScopeContext { get; }
        public ScopeContext ConstructorScopeContext { get; } = new();
        public Dictionary<string, DefinedFunction> DefinedMethods { get; } = new();
        public Dictionary<string, DefinedVariable> DefinedProperties { get; } = new();

        public DefinedClass(ClassDefinition classDefinition, ScopeContext scopeContext) : 
            base(classDefinition.Name, classDefinition.Constructor, classDefinition.Functions, classDefinition.Properties)
        {
            ScopeContext = scopeContext;
        }
    }
}