using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
{
    public class RelativeExpression : IOperatorExpression, IExpression
    {
        public RelativeExpressionType Type { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public RelativeExpression(RelativeExpressionType type, IExpression left, IExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitRelativeExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitRelativeExpression(this, executableScopeContext);
    }
}