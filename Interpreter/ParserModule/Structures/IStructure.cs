using Interpreter.Executor;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures
{
    public interface IStructure
    {
        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor semanticValidatorVisitor);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext);
    }
}