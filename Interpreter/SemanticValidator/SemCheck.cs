using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Errors;
using Interpreter.ParserModule;
using Interpreter.ParserModule.Structures.Definitions;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.ParserModule.Structures.Expressions.Literals;
using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SemanticValidator.Maps;

namespace Interpreter.SemanticValidator
{
    public class SemCheck : IStructuresVisitor
    {
        private static class BuiltStandardTypesNames
        {
            public const string Int = "int";
            public const string Bool = "bool";
            public const string String = "string";
        }

        private static class BuiltSpecialFunctionsTypesNames
        {
            public const string Void = "void";
        }
        
        private const string MainFunctionName = "Main";
        private const string UndefinedType = "undefined";

        private Dictionary<string, DefinedClass> _definedClasses;
        private Dictionary<string, DefinedFunction> _definedFunctions;
        private Dictionary<string, DefinedFunction> _stdFunctions;
        private string _currentFunctionType;

        private bool IsTypeDefined(string type) => 
            type is BuiltStandardTypesNames.Int or BuiltStandardTypesNames.Bool || _definedClasses.ContainsKey(type);

        public void ValidateProgram(ProgramInstance programInstance)
        {
            _definedClasses = new Dictionary<string, DefinedClass>();
            _definedFunctions = new Dictionary<string, DefinedFunction>();

            foreach (var classDefinition in programInstance.Classes.Values)
            {
                ValidateClassDefinitionHeader(classDefinition);
            }
            foreach (var functionDefinition in programInstance.Functions.Values)
            {
                ValidateFunctionDefinitionHeader(functionDefinition);
            }
            ValidateMainFunction();
            foreach (var function in _definedFunctions.Values)
            {
                _currentFunctionType = function.Type;
                var scopeContext = new ScopeContext(function.ScopeContext)
                {
                    DefinedFunctions = _definedFunctions
                };
                ValidateInstructionsBlock(scopeContext, function.Instructions);
            }
        }

        private void ValidateClassDefinitionHeader(ClassDefinition classDefinition)
        {
            if (_definedClasses.ContainsKey(classDefinition.Name))
                ErrorsHandler.HandleError($"ERROR: Redefinition of class '{classDefinition.Name}'");

            if (classDefinition.Name == MainFunctionName)
                ErrorsHandler.HandleError($"ERROR: Class can not be declared with name '{MainFunctionName}'");

            _definedClasses.Add(classDefinition.Name, new DefinedClass(classDefinition, new ScopeContext()));
        }
        
        private void ValidateFunctionDefinitionHeader(FunctionDefinition functionDefinition)
        {
            if (!IsTypeDefined(functionDefinition.Type) && functionDefinition.Type != BuiltSpecialFunctionsTypesNames.Void)
                ErrorsHandler.HandleError($"ERROR: Unknown type '{functionDefinition.Type}' in function {functionDefinition.Name} definition");

            if (_definedClasses.ContainsKey(functionDefinition.Name))
                ErrorsHandler.HandleError($"ERROR: Function '{functionDefinition.Name}' conflicts with already defined class '{functionDefinition.Name}'");

            if (_definedFunctions.ContainsKey(functionDefinition.Name))
                ErrorsHandler.HandleError($"ERROR: Redefinition of function '{functionDefinition.Name}'");

            if (_stdFunctions.ContainsKey(functionDefinition.Name))
                ErrorsHandler.HandleError($"ERROR: Redefinition of standard lib function '{functionDefinition.Name}'");

            var scopeContext = new ScopeContext();
            foreach (var parameter in functionDefinition.Parameters)
            {
                if (!IsTypeDefined(parameter.Type))
                    ErrorsHandler.HandleError($"ERROR: Unknown type '{parameter.Type}' for '{parameter.Name}' parameter in function {functionDefinition.Name} definition");

                if (!scopeContext.DefinedVariables.TryAdd(parameter.Name, new DefinedVariable(parameter.Type, parameter.Name, true)))
                    ErrorsHandler.HandleError($"ERROR: Redefinition of parameter '{parameter.Name} in function '{functionDefinition.Name}' definition");
            }
            _definedFunctions.Add(functionDefinition.Name, new DefinedFunction(functionDefinition, scopeContext));
        }

        private void ValidateMainFunction()
        {
            if (!_definedFunctions.TryGetValue(MainFunctionName, out var mainFunction))
                ErrorsHandler.HandleError($"ERROR: No '{MainFunctionName}' function is defined");

            if (mainFunction != null && mainFunction.Parameters.Any())
                ErrorsHandler.HandleError($"ERROR: Function '{MainFunctionName} can not have parameters");
        }

        private void ValidateInstructionsBlock(ScopeContext scopeContext, IEnumerable<IInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                instruction.Accept(this, scopeContext);
            }
        }

        public void VisitVarDeclarationInstruction(VarDeclaration varDeclaration, ScopeContext scopeContext)
        {
            if (!IsTypeDefined(varDeclaration.Type))
                ErrorsHandler.HandleError($"ERROR: Declaration variable '{varDeclaration.Name}' of unknown type '{varDeclaration.Type}'");

            if (!scopeContext.DefinedVariables.TryAdd(varDeclaration.Name, new DefinedVariable(varDeclaration.Type, varDeclaration.Name, varDeclaration.Value != null)))
                ErrorsHandler.HandleError($"ERROR: Redeclaration of variable '{varDeclaration.Name}'");

            if (varDeclaration.Value == null)
                return;

            var expressionType = varDeclaration.Value.Accept(this, scopeContext);
            if (varDeclaration.Type != expressionType)
                ErrorsHandler.HandleError($"ERROR: Unable assign expression with type '{expressionType}' to variable '{varDeclaration.Name}' with type '{varDeclaration.Type}'");
        }

        public void VisitAssignmentInstruction(Assignment assignment, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(assignment.VariableName, out var variable))
                ErrorsHandler.HandleError($"ERROR: Assignment to undefined variable '{assignment.VariableName}'");
            
            if (variable == null)
                return;

            var expressionType = assignment.Expression.Accept(this, scopeContext);
            if (variable.Type != expressionType)
                ErrorsHandler.HandleError($"ERROR: Unable assign expression with type '{expressionType}' to variable '{variable.Name}' with type '{variable.Type}'");
        }

        public void VisitFunctionCallInstruction(FunctionCall functionCall, ScopeContext scopeContext)
        {
            ValidateFunctionCall(functionCall, scopeContext);
            var functionType = scopeContext.DefinedFunctions[functionCall.Name].Type;
            if (functionType != BuiltSpecialFunctionsTypesNames.Void)
                ErrorsHandler.HandleWarning($"WARNING: Return value from function '{functionCall.Name}' is not used");
        }

        public void VisitMethodCallInstruction(MethodCall methodCall, ScopeContext scopeContext)
        {
            ValidateMethodCall(methodCall, scopeContext);
            var objectType = scopeContext.DefinedVariables[methodCall.ObjectName].Type;
            var methodType = _definedClasses[objectType].Functions[methodCall.Function.Name].Type;
            if (methodType != BuiltSpecialFunctionsTypesNames.Void)
                ErrorsHandler.HandleWarning($"WARNING: Return value from method '{methodCall.Function.Name}' call on object '{methodCall.ObjectName}' is not used");
        }

        public void VisitReturnInstruction(ReturnInstruction returnInstruction, ScopeContext scopeContext)
        {
            var expressionType = returnInstruction.ToReturn == null ? BuiltSpecialFunctionsTypesNames.Void : returnInstruction.ToReturn.Accept(this, scopeContext);
            if (expressionType != _currentFunctionType)
                ErrorsHandler.HandleError($"ERROR: Can not convert expression type '{expressionType}' to return type '{_currentFunctionType}'");
        }

        public void VisitIfInstruction(IfInstruction ifInstruction, ScopeContext scopeContext)
        {
            var conditionType = ifInstruction.Condition.Accept(this, scopeContext);
            if (conditionType != BuiltStandardTypesNames.Bool)
                ErrorsHandler.HandleError($"ERROR: Condition expression has to have '{BuiltStandardTypesNames.Bool}' type");
            ValidateInstructionsBlock(new ScopeContext(scopeContext), ifInstruction.BaseInstructions);
            ValidateInstructionsBlock(new ScopeContext(scopeContext), ifInstruction.ElseInstructions);
        }

        public void VisitWhileInstruction(WhileInstruction whileInstruction, ScopeContext scopeContext)
        {
            var conditionType = whileInstruction.Condition.Accept(this, scopeContext);
            if (conditionType != BuiltStandardTypesNames.Bool)
                ErrorsHandler.HandleError($"ERROR: Condition expression has to have '{BuiltStandardTypesNames.Bool}' type");
            ValidateInstructionsBlock(new ScopeContext(scopeContext), whileInstruction.Instructions);
        }

        public string VisitOrExpression(OrExpression orExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, orExpression);
            if (leftType != BuiltStandardTypesNames.Bool || rightType != BuiltStandardTypesNames.Bool)
                ErrorsHandler.HandleError($"Can not apply operator 'or' to operands of type '{leftType}' and '{rightType}'");
            return BuiltStandardTypesNames.Bool;
        }

        public string VisitAndExpression(AndExpression andExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, andExpression);
            if (leftType != BuiltStandardTypesNames.Bool || rightType != BuiltStandardTypesNames.Bool)
                ErrorsHandler.HandleError($"Can not apply operator 'and' to operands of type '{leftType}' and '{rightType}'");
            return BuiltStandardTypesNames.Bool;
        }

        public string VisitRelativeExpression(RelativeExpression relativeExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, relativeExpression);
            if (leftType != rightType 
                || (leftType != BuiltStandardTypesNames.Bool && leftType != BuiltStandardTypesNames.Int) 
                || (leftType == BuiltStandardTypesNames.Bool && (relativeExpression.Type != RelativeExpressionType.Equal && relativeExpression.Type != RelativeExpressionType.NotEqual)))
            {
                ErrorsHandler.HandleError($"Can not apply operator '{RelativeExpressionTypeToOperator.Map[relativeExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            }

            return BuiltStandardTypesNames.Bool;
        }

        public string VisitAdditiveExpression(AdditiveExpression additiveExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, additiveExpression);
            if (leftType != BuiltStandardTypesNames.Int || rightType != BuiltStandardTypesNames.Int)
                ErrorsHandler.HandleError($"Can not apply operator '{AdditiveExpressionTypeToOperator.Map[additiveExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            return BuiltStandardTypesNames.Int;
        }

        public string VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, multiplicativeExpression);
            if (leftType != BuiltStandardTypesNames.Int || rightType != BuiltStandardTypesNames.Int)
                ErrorsHandler.HandleError($"Can not apply operator '{MultiplicativeExpressionTypeToOperator.Map[multiplicativeExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            return BuiltStandardTypesNames.Int;
        }

        public string VisitNotExpression(NotExpression notExpression, ScopeContext scopeContext)
        {
            var negatedType = notExpression.Left.Accept(this, scopeContext);
            if (negatedType!= BuiltStandardTypesNames.Bool)
                ErrorsHandler.HandleError($"Can not apply operator 'not' to operand of type '{negatedType}'");
            return BuiltStandardTypesNames.Bool;
        }

        public string VisitVariableExpression(VariableExpression variableExpression, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(variableExpression.Name, out var variable))
                ErrorsHandler.HandleError($"ERROR: Usage of undefined variable '{variableExpression.Name}'");

            if (variable == null)
                return string.Empty;

            if (!variable.IsInitialized)
                ErrorsHandler.HandleError($"ERROR: Usage of uninitialized variable '{variableExpression.Name}'");

            return variable.Type;
        }

        public string VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(propertyCallExpression.ObjectName, out var objectVariable))
                ErrorsHandler.HandleError($"ERROR: Use of undefined variable {propertyCallExpression.ObjectName}");

            if (objectVariable == null)
                return string.Empty;
            
            if (!_definedClasses.ContainsKey(objectVariable.Type))
                ErrorsHandler.HandleError($"ERROR: Variable '{objectVariable.Name}' is not an object and does not support '.' operator");

            var variableClass = _definedClasses[objectVariable.Type];
            if (!variableClass.Properties.TryGetValue(propertyCallExpression.PropertyName, out var propertyDeclaration))
                ErrorsHandler.HandleError($"ERROR: Can not resolve property '{propertyCallExpression.PropertyName}' for variable '{objectVariable.Name}' with type '{objectVariable.Type}");

            return propertyDeclaration == null ? string.Empty : propertyDeclaration.Type;
        }

        public string VisitMethodCallExpression(MethodCall methodCall, ScopeContext scopeContext)
        {
            ValidateMethodCall(methodCall, scopeContext);
            var variableType = scopeContext.DefinedVariables[methodCall.ObjectName].Type;
            return _definedClasses[variableType].Functions[methodCall.Function.Name].Type;
        }

        public string VisitFunctionCallExpression(FunctionCall functionCall, ScopeContext scopeContext)
        {
            ValidateFunctionCall(functionCall, scopeContext);
            return scopeContext.DefinedFunctions[functionCall.Name].Type;
        }

        public string VisitIntLiteralExpression(IntLiteral intLiteral, ScopeContext scopeContext)
        {
            return BuiltStandardTypesNames.Int;
        }

        public string VisitBoolLiteralExpression(BoolLiteral boolLiteral, ScopeContext scopeContext)
        {
            return BuiltStandardTypesNames.Bool;
        }

        public string VisitStringLiteralExpression(StringLiteral stringLiteral, ScopeContext scopeContext)
        {
            return BuiltStandardTypesNames.String;
        }

        private void ValidateFunctionCall(FunctionCall functionCall, ScopeContext scopeContext)
        {
            if (!_definedClasses.ContainsKey(functionCall.Name) && !scopeContext.DefinedFunctions.ContainsKey(functionCall.Name) && ! _stdFunctions.ContainsKey(functionCall.Name))
                ErrorsHandler.HandleError($"ERROR: Call undefined function '{functionCall.Name}'");

            FunctionDefinition calledFunction = null;
            if (_definedClasses.ContainsKey(functionCall.Name))
                calledFunction = _definedClasses[functionCall.Name].Constructor;
            else if (scopeContext.DefinedFunctions.ContainsKey(functionCall.Name))
                calledFunction = scopeContext.DefinedFunctions[functionCall.Name];
            else if (_stdFunctions.ContainsKey(functionCall.Name)) calledFunction = _stdFunctions[functionCall.Name];
            if (calledFunction == null)
                throw new Exception("Unexpected error");
            ValidateCallArguments(scopeContext, functionCall, scopeContext.DefinedFunctions[functionCall.Name]);
        }

        private void ValidateMethodCall(MethodCall methodCall, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(methodCall.ObjectName, out var objectVariable))
                ErrorsHandler.HandleError($"ERROR: Use of undefined variable {methodCall.ObjectName}");

            if (objectVariable == null)
                throw new Exception("Unexpected error");
            
            if (!scopeContext.DefinedFunctions.ContainsKey(objectVariable.Type))
                ErrorsHandler.HandleError($"ERROR: Variable '{objectVariable.Name}' is not an object and does not support '.' operator");

            var variableClass = _definedClasses[objectVariable.Type];
            if (!variableClass.Functions.ContainsKey(methodCall.Function.Name))
                ErrorsHandler.HandleError($"ERROR: Unable to call method '{methodCall.Function.Name}' for variable '{objectVariable.Name}' with type '{objectVariable.Type}'");
            ValidateCallArguments(scopeContext, methodCall.Function, scopeContext.DefinedFunctions[methodCall.Function.Name], true);
        }
        
        private void ValidateCallArguments(ScopeContext scopeContext, FunctionCall functionCall, FunctionDefinition functionDefinition, bool isMethod = false)
        {
            var functionParameters = functionDefinition.Parameters.ToList();
            var functionArguments = functionCall.Arguments.ToList();

            if (functionParameters.Count != functionArguments.Count)
                ErrorsHandler.HandleError($"ERROR: Function '{functionDefinition.Name} has {functionParameters.Count} parameter(s) but is invoked with {functionArguments.Count} argument(s)");

            var callType = isMethod ? "method" : "function";
            for (var i = 0; i < functionArguments.Count; i++)
            {
                var parameterType = functionParameters[i].Type;
                var argumentType = functionArguments[i].Accept(this, scopeContext);
                if (parameterType != argumentType)
                    ErrorsHandler.HandleError($"ERROR: Argument type '{argumentType}' is not assignable to parameter type '{parameterType}' in {callType} '{functionCall.Name}' call");
            }
        }

        private (string leftExpressionType, string rightExpressionType) ValidateOperatorExpressions(ScopeContext scopeContext, IOperatorExpression expression)
        {
            return (expression.Left.Accept(this, scopeContext), expression.Left.Accept(this, scopeContext));
        }

        private void ValidateClassDefinition(DefinedClass definedClass)
        {
            foreach (var property in definedClass.Properties.Values)
            {
                if (!IsTypeDefined(property.Type))
                    ErrorsHandler.HandleError($"ERROR: Declaration property '{property.Name}' of unknown type '{property.Type}' in class '{definedClass.Name}' definition");

                if (definedClass.DefinedProperties.ContainsKey(property.Name))
                    ErrorsHandler.HandleError($"ERROR: Redefinition of property '{property.Name} in class '{definedClass.Name}' definition");

                if (property.Value != null)
                {
                    var expressionType = property.Value.Accept(this, new ScopeContext());
                    if (property.Type != expressionType)
                        ErrorsHandler.HandleError($"ERROR: Unable assign expression with type '{expressionType}' to property '{property.Name}' with type '{property.Type}' in class '{definedClass.Name}' definition");
                }
                definedClass.DefinedProperties.TryAdd(property.Name, new DefinedVariable(property.Type, property.Name, true));
            }

            foreach (var methodDefinition in definedClass.Functions.Values)
            {
                if (!IsTypeDefined(methodDefinition.Type) && methodDefinition.Type != BuiltSpecialFunctionsTypesNames.Void)
                    ErrorsHandler.HandleError($"ERROR: Unknown type '{methodDefinition.Type}' in method {methodDefinition.Name} definition in '{definedClass.Name}' class");

                if (definedClass.DefinedMethods.ContainsKey(methodDefinition.Name))
                    ErrorsHandler.HandleError($"ERROR: Redefinition of method '{methodDefinition.Name}' in '{definedClass.Name}' class");

                var methodScopeContext = new ScopeContext(new Dictionary<string, DefinedFunction>(), definedClass.DefinedProperties);
                foreach (var parameter in methodDefinition.Parameters)
                {
                    if (!IsTypeDefined(parameter.Type))
                        ErrorsHandler.HandleError($"ERROR: Unknown type '{parameter.Type}' for '{parameter.Name}' parameter in method {methodDefinition.Name} definition in '{definedClass.Name}' class");
                    
                    if(!definedClass.DefinedProperties.ContainsKey(parameter.Name))
                        ErrorsHandler.HandleError($"ERROR: Parameter's '{parameter.Name} conflicts with property in method {methodDefinition.Name} definition in '{definedClass.Name}' class");

                    if (!methodScopeContext.DefinedVariables.TryAdd(parameter.Name, new DefinedVariable(parameter.Type, parameter.Name, true)))
                        ErrorsHandler.HandleError($"ERROR: Redefinition of parameter '{parameter.Name} in function '{methodDefinition.Name}' definition");
                }
                definedClass.DefinedMethods.Add(methodDefinition.Name, new DefinedFunction(methodDefinition, methodScopeContext));
            }

            foreach (var constructorParameter in definedClass.Constructor.Parameters)
            {
                if (!IsTypeDefined(constructorParameter.Type))
                    ErrorsHandler.HandleError($"ERROR: Unknown type '{constructorParameter.Type}' for '{constructorParameter.Name}' parameter in {definedClass.Name} constructor definition");

                if(!definedClass.DefinedProperties.ContainsKey(constructorParameter.Name))
                    ErrorsHandler.HandleError($"ERROR: Parameter's '{constructorParameter.Name} conflicts with property in '{definedClass.Name}' constructor definition");

                if (!definedClass.ConstructorScopeContext.DefinedVariables.TryAdd(constructorParameter.Name, new DefinedVariable(constructorParameter.Type, constructorParameter.Name, true)))
                    ErrorsHandler.HandleError($"ERROR: Redefinition of parameter '{constructorParameter.Name} in '{definedClass.Name}' constructor definition");
            }

            var variables = definedClass.DefinedProperties.Values.ToList().Concat(definedClass.ConstructorScopeContext.DefinedVariables.Values.ToList());
            ValidateInstructionsBlock(new ScopeContext(definedClass.DefinedMethods, variables.ToDictionary(x => x.Name, x => x)), definedClass.Constructor.Instructions);

            foreach (var method in definedClass.DefinedMethods.Values)
            {
                _currentFunctionType = method.Type;
                var scopeContext = new ScopeContext(method.ScopeContext)
                {
                    DefinedFunctions = _definedFunctions
                };
                ValidateInstructionsBlock(scopeContext, method.Instructions);
            }
        }
    }
}