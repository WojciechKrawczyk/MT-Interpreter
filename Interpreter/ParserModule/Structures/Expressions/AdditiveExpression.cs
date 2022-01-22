using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class AdditiveExpression : IOperatorExpression, IExpression
    {
        public AdditiveExpressionType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public AdditiveExpression(AdditiveExpressionType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitAdditiveExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitAdditiveExpression(this, executableScopeContext);
    }
}