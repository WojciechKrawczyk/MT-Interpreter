using System.Collections.Generic;
using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class FunctionCallExpression : FunctionCall, IExpression
    {
        public FunctionCallExpression(string name, IEnumerable<IExpression> arguments) : base(name, arguments)
        {
        }

        public new string AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitFunctionCallExpression(this, scopeContext);

        public new ExecutableVariable AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitFunctionCallExpression(this, executableScopeContext);
    }
}