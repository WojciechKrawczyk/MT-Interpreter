using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Instructions
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