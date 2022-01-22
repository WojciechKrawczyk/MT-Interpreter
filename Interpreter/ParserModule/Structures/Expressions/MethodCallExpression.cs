using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class MethodCallExpression : MethodCall, IExpression
    {
        public MethodCallExpression(string objectName, FunctionCall function) : base(objectName, function)
        {
        }

        public new string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitMethodCallExpression(this, scopeContext);

        public new ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitMethodCallExpression(this, executableScopeContext);
    }
}