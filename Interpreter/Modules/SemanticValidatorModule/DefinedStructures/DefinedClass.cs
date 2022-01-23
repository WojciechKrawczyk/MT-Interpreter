using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Definitions;

namespace Interpreter.Modules.SemanticValidatorModule.DefinedStructures
{
    public class DefinedClass : ClassDefinition
    {
        public Dictionary<string, DefinedVariable> DefinedProperties { get; } = new();
        public Dictionary<string, DefinedFunction> DefinedMethods { get; } = new();
        public ScopeContext ConstructorScopeContext { get; } = new();

        public DefinedClass(ClassDefinition classDefinition, Dictionary<string, DefinedVariable> definedProperties, Dictionary<string, DefinedFunction> definedMethods, ScopeContext constructorScopeContext) : 
            base(classDefinition.Name, classDefinition.Constructor, classDefinition.Functions, classDefinition.Properties)
        {
            DefinedProperties = definedProperties;
            DefinedMethods = definedMethods;
            ConstructorScopeContext = constructorScopeContext;
        }
    }
}