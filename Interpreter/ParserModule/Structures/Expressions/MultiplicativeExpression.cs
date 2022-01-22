using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class MultiplicativeExpression : IOperatorExpression, IExpression
    {
        public MultiplicativeExpressionType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public MultiplicativeExpression(MultiplicativeExpressionType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }
        
        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitMultiplicativeExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitMultiplicativeExpression(this, executableScopeContext);
    }
}