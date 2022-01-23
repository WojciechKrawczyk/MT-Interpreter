using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.ParserModule.Structures.Expressions.Literals;
using Interpreter.Modules.ParserModule.Structures.Instructions;

namespace Interpreter.Executor
{
    public interface IStructuresExecutorVisitor
    { 
        public void VisitVarDeclarationInstruction(VarDeclaration varDeclaration, ExecutableScopeContext executableScopeContext);
        public void VisitAssignmentInstruction(Assignment assignment, ExecutableScopeContext executableScopeContext);
        public void VisitFunctionCallInstruction(FunctionCall functionCall, ExecutableScopeContext executableScopeContext);
        public void VisitMethodCallInstruction(MethodCall methodCall, ExecutableScopeContext executableScopeContext);
        public void VisitReturnInstruction(ReturnInstruction returnInstruction, ExecutableScopeContext executableScopeContext);
        public void VisitIfInstruction(IfInstruction ifInstruction, ExecutableScopeContext executableScopeContext);
        public void VisitWhileInstruction(WhileInstruction whileInstruction, ExecutableScopeContext executableScopeContext);

        public ExecutableVariable VisitOrExpression(OrExpression orExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitAndExpression(AndExpression andExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitRelativeExpression(RelativeExpression relativeExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitAdditiveExpression(AdditiveExpression additiveExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitNotExpression(NotExpression notExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitVariableExpression(VariableExpression variableExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitMethodCallExpression(MethodCall methodCall, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitFunctionCallExpression(FunctionCall functionCall, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitIntLiteralExpression(IntLiteral intLiteral, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitBoolLiteralExpression(BoolLiteral boolLiteral, ExecutableScopeContext executableScopeContext);
        public ExecutableVariable VisitStringLiteralExpression(StringLiteral stringLiteral, ExecutableScopeContext executableScopeContext);
    }
}