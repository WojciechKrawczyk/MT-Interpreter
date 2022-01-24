using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
{
    public class NotExpression : IOperatorExpression, IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public NotExpression(IExpression expression)
        {
            Left = expression;
            Right = null;
        }
        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitNotExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitNotExpression(this, executableScopeContext);
    }
}