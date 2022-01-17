using Interpreter.ParserModule.Structures.Definitions;

namespace Interpreter.SemanticValidator
{
    public class DefinedClass : ClassDefinition
    {
        public ScopeContext ScopeContext { get; }

        public DefinedClass(ClassDefinition classDefinition, ScopeContext scopeContext) : 
            base(classDefinition.Name, classDefinition.Constructor, classDefinition.Functions, classDefinition.Properties)
        {
            ScopeContext = scopeContext;
        }
    }
}