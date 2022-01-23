using Interpreter.Executor;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures
{
    public interface IStructure
    {
        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor semanticValidatorVisitor);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext);
    }
}