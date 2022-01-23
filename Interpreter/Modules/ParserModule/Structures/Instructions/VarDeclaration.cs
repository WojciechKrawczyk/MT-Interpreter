using Interpreter.Executor;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.SemanticValidatorModule;

namespace Interpreter.Modules.ParserModule.Structures.Instructions
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

        public void AcceptSemanticValidator(IStructuresSemanticValidatorVisitor structuresSemanticValidatorVisitor, ScopeContext scopeContext) => 
            structuresSemanticValidatorVisitor.VisitVarDeclarationInstruction(this, scopeContext);

        public void AcceptExecutor(IStructuresExecutorVisitor structuresExecutorVisitor, ExecutableScopeContext executableScopeContext) =>
            structuresExecutorVisitor.VisitVarDeclarationInstruction(this, executableScopeContext);
    }
}