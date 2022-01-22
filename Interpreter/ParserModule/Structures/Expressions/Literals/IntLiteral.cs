using Interpreter.Executor;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class IntLiteral : IExpression
    {
        public int Value { get; }

        public IntLiteral(int value)
        {
            Value = value;
        }

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitIntLiteralExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitIntLiteralExpression(this, executableScopeContext);
    }
}