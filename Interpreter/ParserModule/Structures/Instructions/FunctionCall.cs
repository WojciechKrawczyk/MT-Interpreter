using System.Collections.Generic;
using Interpreter.Executor;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class FunctionCall : IInstruction
    {
        public string Name { get; }
        public IEnumerable<IExpression> Arguments { get; }

        public FunctionCall(string name, IEnumerable<IExpression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitFunctionCallInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) => 
            structuresExecutorVisitor.VisitFunctionCallInstruction(this, executableScopeContext);
    }
}