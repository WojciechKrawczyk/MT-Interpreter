using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
{
    public class OrExpression : IOperatorExpression, IExpression
    {
        
        public IExpression Left { get; }
        public IExpression Right { get; }

        public OrExpression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitOrExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitOrExpression(this, executableScopeContext);
    }
}