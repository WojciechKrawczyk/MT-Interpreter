using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
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