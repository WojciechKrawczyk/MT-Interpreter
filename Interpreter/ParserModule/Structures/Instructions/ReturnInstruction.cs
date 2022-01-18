using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class ReturnInstruction : IInstruction
    {
        public IExpression ToReturn { get; }

        public ReturnInstruction(IExpression toReturn)
        {
            ToReturn = toReturn;
        }

        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitReturnInstruction(this, scopeContext);
    }
}