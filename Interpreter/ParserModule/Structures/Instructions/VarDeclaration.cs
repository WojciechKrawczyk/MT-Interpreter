using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.SemanticValidator;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class VarDeclaration : IInstruction
    {
        public string Name { get; }
        public string Type { get; }
        public IExpression Value { get; }

        public VarDeclaration(string name, string type, IExpression value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public void Accept(IStructuresVisitor structuresVisitor, ScopeContext scopeContext) => 
            structuresVisitor.VisitVarDeclarationInstruction(this, scopeContext);
    }
}