using Interpreter.Executor;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class MethodCall : IInstruction
    {
        public string ObjectName { get; }
        public FunctionCall Function { get; }

        public MethodCall(string objectName, FunctionCall function)
        {
            ObjectName = objectName;
            Function = function;
        }

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitMethodCallInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitMethodCallInstruction(this, executableScopeContext);
    }
}