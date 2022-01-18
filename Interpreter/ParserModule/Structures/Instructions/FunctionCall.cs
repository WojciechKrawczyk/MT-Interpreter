using System.Collections.Generic;
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

        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitFunctionCallInstruction(this, scopeContext);
    }
}