using System.Collections.Generic;
using Interpreter.Modules.ExecutorModule;
using Interpreter.Modules.ParserModule.Structures.Instructions;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Expressions
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