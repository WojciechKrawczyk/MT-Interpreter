using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions.Literals
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