using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.ParserModule.Structures.Expressions.Literals;
using Interpreter.ParserModule.Structures.Instructions;

namespace Interpreter.SemanticValidator
{
    public interface IStructuresVisitor
    {
        public void VisitVarDeclarationInstruction(VarDeclaration varDeclaration, ScopeContext scopeContext);
        public void VisitAssignmentInstruction(Assignment assignment, ScopeContext scopeContext);
        public void VisitFunctionCallInstruction(FunctionCall functionCall, ScopeContext scopeContext);
        public void VisitMethodCallInstruction(MethodCall methodCall, ScopeContext scopeContext);
        public void VisitReturnInstruction(ReturnInstruction returnInstruction, ScopeContext scopeContext);
        public void VisitIfInstruction(IfInstruction ifInstruction, ScopeContext scopeContext);
        public void VisitWhileInstruction(WhileInstruction whileInstruction, ScopeContext scopeContext);

        public string VisitOrExpression(OrExpression orExpression, ScopeContext scopeContext);
        public string VisitAndExpression(AndExpression andExpression, ScopeContext scopeContext);
        public string VisitRelativeExpression(RelativeExpression relativeExpression, ScopeContext scopeContext);
        public string VisitAdditiveExpression(AdditiveExpression additiveExpression, ScopeContext scopeContext);
        public string VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression, ScopeContext scopeContext);
        public string VisitNotExpression(NotExpression notExpression, ScopeContext scopeContext);
        public string VisitVariableExpression(VariableExpression variableExpression, ScopeContext scopeContext);
        public string VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression, ScopeContext scopeContext);
        public string VisitMethodCallExpression(MethodCall methodCall, ScopeContext scopeContext);
        public string VisitFunctionCallExpression(FunctionCall functionCall, ScopeContext scopeContext);
        public string VisitIntLiteralExpression(IntLiteral intLiteral, ScopeContext scopeContext);
        public string VisitBoolLiteralExpression(BoolLiteral boolLiteral, ScopeContext scopeContext);
        public string VisitStringLiteralExpression(StringLiteral stringLiteral, ScopeContext scopeContext);
    }
}