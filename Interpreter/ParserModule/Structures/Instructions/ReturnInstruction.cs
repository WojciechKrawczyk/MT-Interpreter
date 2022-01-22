using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class ReturnInstruction : IInstruction
    {
        public IExpression ToReturn { get; }

        public ReturnInstruction(IExpression toReturn)
        {
            ToReturn = toReturn;
        }

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitReturnInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitReturnInstruction(this, executableScopeContext);
    }
}