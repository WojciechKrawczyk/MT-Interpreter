using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Expressions
{
    public class FunctionCallExpression : FunctionCall, IExpression
    {
        public FunctionCallExpression(string name, IEnumerable<IExpression> arguments) : base(name, arguments)
        {
        }

        public new string Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitFunctionCallExpression(this, scopeContext);
    }
}