using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class WhileInstruction : IInstruction
    {
        public IExpression Condition { get; }
        public IEnumerable<IInstruction> Instructions { get; }
        
        public WhileInstruction(IExpression condition, IEnumerable<IInstruction> instructions)
        {
            Condition = condition;
            Instructions = instructions;
        }

        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitWhileInstruction(this, scopeContext);
    }
}