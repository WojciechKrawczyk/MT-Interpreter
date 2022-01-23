using Interpreter.Executor;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions.Literals
{
    public class StringLiteral : IExpression
    {
        public string Value { get; }

        public StringLiteral(string value)
        {
            Value = value;
        }

        public string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitStringLiteralExpression(this, scopeContext);

        public ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitStringLiteralExpression(this, executableScopeContext);
    }
}