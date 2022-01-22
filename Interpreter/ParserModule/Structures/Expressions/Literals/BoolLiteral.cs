using Interpreter.Executor;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class BoolLiteral : IExpression
    {
        public bool Value { get; }

        public BoolLiteral(bool value)
        {
            Value = value;
        }

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitBoolLiteralExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitBoolLiteralExpression(this, executableScopeContext);
    }
}