using Interpreter.Executor;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class AndExpression : IOperatorExpression, IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public AndExpression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitAndExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitAndExpression(this, executableScopeContext);
    }
}