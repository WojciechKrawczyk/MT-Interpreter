using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Errors;
using Interpreter.Modules.ParserModule;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.ParserModule.Structures.Expressions.Literals;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;
using Interpreter.Modules.ParserModule.Structures.Instructions;
using Interpreter.Modules.SemanticValidatorModule.DefinedStructures;
using Interpreter.Modules.SemanticValidatorModule.Maps;
using Interpreter.Modules.SemanticValidatorModule.ValidStructures;
using Interpreter.Modules.StdResources;

namespace Interpreter.Modules.SemanticValidatorModule
{
    public class SemanticValidator : IStructuresSemanticValidatorVisitor
    {
        private Dictionary<string, DefinedClass> _definedClasses;
        private Dictionary<string, DefinedFunction> _definedFunctions;
        private readonly Dictionary<string, DefinedFunction> _stdFunctions = StdFunctions.Functions;
        private readonly ErrorsHandler _errorsHandler;
        private string _currentFunctionType;

        private bool IsTypeDefined(string type) => 
            type is StdTypesNames.Int or StdTypesNames.Bool || _definedClasses.ContainsKey(type);

        public SemanticValidator(ErrorsHandler errorsHandler)
        {
            _errorsHandler = errorsHandler;
        }

        public ValidProgramInstance ValidateProgram(ProgramInstance programInstance)
        {
            _definedClasses = new Dictionary<string, DefinedClass>();
            _definedFunctions = new Dictionary<string, DefinedFunction>();

            foreach (var classDefinition in programInstance.Classes)
            {
                ValidateClassDefinitionHeader(classDefinition);
            }
            foreach (var functionDefinition in programInstance.Functions)
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

            foreach (var definedCLass in _definedClasses.Values)
            {
                ValidateClassDefinition(definedCLass);
            }

            if (_errorsHandler.Errors.Any())
                _errorsHandler.StopInterpretation();

            return new ValidProgramInstance
            {
                Functions = _definedFunctions.Values
                    .Select(x => new ValidFunction(x.Name, x.Type, x.Parameters, x.Instructions))
                    .ToDictionary(x => x.Name, x => x),
                Classes = _definedClasses.Values
                    .Select(x => new ValidClass
                    {
                        Name = x.Name,
                        Constructor = x.Constructor,
                        Methods = x.DefinedMethods.Values
                            .Select(y => new ValidFunction(y.Name, y.Type, y.Parameters, y.Instructions))
                            .ToDictionary(y => y.Name, y => y),
                        Properties = x.DefinedProperties.Values.
                            Select(y => x.Properties.First(z => z.Name == y.Name))
                            .ToDictionary(y => y.Name, y=> y)
                    })
                    .ToDictionary(x => x.Name, x => x)
            };
        }

        private void ValidateClassDefinitionHeader(ClassDefinition classDefinition)
        {
            var isClassOk = true;
            if (_definedClasses.ContainsKey(classDefinition.Name))
                _errorsHandler.HandleError($"Redefinition of class '{classDefinition.Name}'");

            if (classDefinition.Name == StdNames.MainFunctionName)
            {
                _errorsHandler.HandleError($"Class can not be defined with name '{StdNames.MainFunctionName}'");
                isClassOk = false;
            }

            var definedProperties = new Dictionary<string, DefinedVariable>();
            foreach (var property in classDefinition.Properties)
            {
                if (!IsTypeDefined(property.Type))
                    _errorsHandler.HandleError($"Declaration property '{property.Name}' of unknown type '{property.Type}' in class '{classDefinition.Name}' definition");

                if (definedProperties.ContainsKey(property.Name))
                    _errorsHandler.HandleError($"Redefinition of property '{property.Name}' in class '{classDefinition.Name}' definition");

                if (property.Value != null)
                {
                    var expressionType = property.Value.AcceptSemanticValidator(this, new ScopeContext());
                    if (property.Type != expressionType)
                    {
                        _errorsHandler.HandleError($"Unable assign expression with type '{expressionType}' to property '{property.Name}' with type '{property.Type}' in class '{classDefinition.Name}' definition");
                    }
                }
                definedProperties.TryAdd(property.Name, new DefinedVariable(property.Type, property.Name, property.Value != null));
            }

            var definedMethods = new Dictionary<string, DefinedFunction>();
            foreach (var methodDefinition in classDefinition.Functions)
            {
                if (!IsTypeDefined(methodDefinition.Type) && methodDefinition.Type != StdTypesNames.Void)
                    _errorsHandler.HandleError($"Unknown type '{methodDefinition.Type}' in method '{methodDefinition.Name}' definition in '{classDefinition.Name}' class");

                if (definedMethods.ContainsKey(methodDefinition.Name))
                    _errorsHandler.HandleError($"Redefinition of method '{methodDefinition.Name}' in '{classDefinition.Name}' class");
                
                if (_stdFunctions.ContainsKey(methodDefinition.Name))
                    _errorsHandler.HandleError($"Redefinition of std function '{methodDefinition.Name}' in '{classDefinition.Name}' class");

                var methodScopeContext = new ScopeContext();
                foreach (var parameter in methodDefinition.Parameters)
                {
                    if (!IsTypeDefined(parameter.Type))
                        _errorsHandler.HandleError($"Unknown type '{parameter.Type}' for '{parameter.Name}' parameter in method '{methodDefinition.Name}' definition in '{classDefinition.Name}' class");
                    
                    if(definedProperties.ContainsKey(parameter.Name))
                        _errorsHandler.HandleError($"Parameter '{parameter.Name}' conflicts with property in method '{methodDefinition.Name}' definition in '{classDefinition.Name}' class");

                    if (!methodScopeContext.DefinedVariables.TryAdd(parameter.Name, new DefinedVariable(parameter.Type, parameter.Name, true)))
                        _errorsHandler.HandleError($"Redefinition of parameter '{parameter.Name}' in method '{methodDefinition.Name}' definition");
                }
                definedMethods.TryAdd(methodDefinition.Name, new DefinedFunction(methodDefinition, methodScopeContext));
            }
            foreach (var method in definedMethods.Values)
            {
                method.ScopeContext.DefinedFunctions = definedMethods;
            }

            var constructorScopeContext = new ScopeContext();
            foreach (var constructorParameter in classDefinition.Constructor.Parameters)
            {
                if (!IsTypeDefined(constructorParameter.Type))
                    _errorsHandler.HandleError($"Unknown type '{constructorParameter.Type}' for '{constructorParameter.Name}' parameter in constructor definition in '{classDefinition.Name}' class");
                
                if(definedProperties.ContainsKey(constructorParameter.Name))
                    _errorsHandler.HandleError($"Parameter '{constructorParameter.Name}' conflicts with property in constructor definition in '{classDefinition.Name}' class");

                if (constructorScopeContext.DefinedVariables.ContainsKey(constructorParameter.Name))
                    _errorsHandler.HandleError($"Redefinition of parameter '{constructorParameter.Name}' in constructor definition in '{classDefinition.Name}' class");

                if (!definedProperties.ContainsKey(constructorParameter.Name) && !constructorScopeContext.DefinedVariables.ContainsKey(constructorParameter.Name))
                    constructorScopeContext.DefinedVariables.Add(constructorParameter.Name, new DefinedVariable(constructorParameter.Type, constructorParameter.Name, true));
            }

            if(isClassOk)
                _definedClasses.TryAdd(classDefinition.Name, new DefinedClass(classDefinition, definedProperties, definedMethods, constructorScopeContext));
        }
        
        private void ValidateFunctionDefinitionHeader(FunctionDefinition functionDefinition)
        {
            if (!IsTypeDefined(functionDefinition.Type) && functionDefinition.Type != StdTypesNames.Void)
                _errorsHandler.HandleError($"Unknown type '{functionDefinition.Type}' in function '{functionDefinition.Name}' definition");

            var isRedefinition = true;
            if (_definedClasses.ContainsKey(functionDefinition.Name))
                _errorsHandler.HandleError($"Function '{functionDefinition.Name}' conflicts with already defined class '{functionDefinition.Name}'");
            else if (_definedFunctions.ContainsKey(functionDefinition.Name))
                _errorsHandler.HandleError($"Redefinition of function '{functionDefinition.Name}'");
            else if (_stdFunctions.ContainsKey(functionDefinition.Name))
                _errorsHandler.HandleError($"Redefinition of standard lib function '{functionDefinition.Name}'");
            else
                isRedefinition = false;

            var scopeContext = new ScopeContext();
            foreach (var parameter in functionDefinition.Parameters)
            {
                if (!IsTypeDefined(parameter.Type))
                    _errorsHandler.HandleError($"Unknown type '{parameter.Type}' for '{parameter.Name}' parameter in function '{functionDefinition.Name}' definition");

                if (!scopeContext.DefinedVariables.TryAdd(parameter.Name, new DefinedVariable(parameter.Type, parameter.Name, true)))
                    _errorsHandler.HandleError($"Redefinition of parameter '{parameter.Name}' in function '{functionDefinition.Name}' definition");
            }
            if (!isRedefinition)
                _definedFunctions.Add(functionDefinition.Name, new DefinedFunction(functionDefinition, scopeContext));
        }

        private void ValidateMainFunction()
        {
            if (!_definedFunctions.TryGetValue(StdNames.MainFunctionName, out var mainFunction))
                _errorsHandler.HandleError($"No '{StdNames.MainFunctionName}' function is defined");

            if (mainFunction != null && mainFunction.Type != StdTypesNames.Void)
                _errorsHandler.HandleError($"Function '{StdNames.MainFunctionName} have to have '{StdTypesNames.Void}' type");

            if (mainFunction != null && mainFunction.Parameters.Any())
                _errorsHandler.HandleError($"Function '{StdNames.MainFunctionName} can not have parameters");
        }

        private void ValidateInstructionsBlock(ScopeContext scopeContext, IEnumerable<IInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                instruction.AcceptSemanticValidator(this, scopeContext);
            }
        }

        public void VisitVarDeclarationInstruction(VarDeclaration varDeclaration, ScopeContext scopeContext)
        {
            if (!IsTypeDefined(varDeclaration.Type))
                _errorsHandler.HandleError($"Declaration variable '{varDeclaration.Name}' of unknown type '{varDeclaration.Type}'");

            if (!scopeContext.DefinedVariables.TryAdd(varDeclaration.Name, new DefinedVariable(varDeclaration.Type, varDeclaration.Name, varDeclaration.Value != null)))
                _errorsHandler.HandleError($"Redeclaration of variable '{varDeclaration.Name}'");

            if (varDeclaration.Value == null)
                return;

            var expressionType = varDeclaration.Value.AcceptSemanticValidator(this, scopeContext);
            if (varDeclaration.Type != expressionType)
                _errorsHandler.HandleError($"Unable assign expression with type '{expressionType}' to variable '{varDeclaration.Name}' with type '{varDeclaration.Type}'");
        }

        public void VisitAssignmentInstruction(Assignment assignment, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(assignment.VariableName, out var variable))
                _errorsHandler.HandleError($"Assignment to undefined variable '{assignment.VariableName}'");
            
            if (variable == null)
                return;

            var expressionType = assignment.Expression.AcceptSemanticValidator(this, scopeContext);
            if (variable.Type != expressionType)
                _errorsHandler.HandleError($"Unable assign expression with type '{expressionType}' to variable '{variable.Name}' with type '{variable.Type}'");
        }

        public void VisitFunctionCallInstruction(FunctionCall functionCall, ScopeContext scopeContext)
        {
            var isDefined = ValidateFunctionCall(functionCall, scopeContext);
            if (!isDefined)
                return;
            var functionType = scopeContext.DefinedFunctions[functionCall.Name].Type;
            if (functionType != StdTypesNames.Void)
                _errorsHandler.HandleWarning($"Return value from function '{functionCall.Name}' is not used");
        }

        public void VisitMethodCallInstruction(MethodCall methodCall, ScopeContext scopeContext)
        {
            var isOk = ValidateMethodCall(methodCall, scopeContext);
            if (!isOk)
                return;
            var objectType = scopeContext.DefinedVariables[methodCall.ObjectName].Type;
            var methodType = _definedClasses[objectType].DefinedMethods[methodCall.Function.Name].Type;
            if (methodType != StdTypesNames.Void)
                _errorsHandler.HandleWarning($"Return value from method '{methodCall.Function.Name}' called on object '{methodCall.ObjectName}' is not used");
        }

        public void VisitReturnInstruction(ReturnInstruction returnInstruction, ScopeContext scopeContext)
        {
            var expressionType = returnInstruction.ToReturn == null ? StdTypesNames.Void : returnInstruction.ToReturn.AcceptSemanticValidator(this, scopeContext);
            if (expressionType != _currentFunctionType)
                _errorsHandler.HandleError($"Can not convert expression type '{expressionType}' to return function type '{_currentFunctionType}'");
        }

        public void VisitIfInstruction(IfInstruction ifInstruction, ScopeContext scopeContext)
        {
            var conditionType = ifInstruction.Condition.AcceptSemanticValidator(this, scopeContext);
            if (conditionType != StdTypesNames.Bool)
                _errorsHandler.HandleError($"Condition expression has to have '{StdTypesNames.Bool}' type");
            ValidateInstructionsBlock(new ScopeContext(scopeContext), ifInstruction.BaseInstructions);
            if (ifInstruction.ElseInstructions != null)
                ValidateInstructionsBlock(new ScopeContext(scopeContext), ifInstruction.ElseInstructions);
        }

        public void VisitWhileInstruction(WhileInstruction whileInstruction, ScopeContext scopeContext)
        {
            var conditionType = whileInstruction.Condition.AcceptSemanticValidator(this, scopeContext);
            if (conditionType != StdTypesNames.Bool)
                _errorsHandler.HandleError($"Condition expression has to have '{StdTypesNames.Bool}' type");
            ValidateInstructionsBlock(new ScopeContext(scopeContext), whileInstruction.Instructions);
        }

        public string VisitOrExpression(OrExpression orExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, orExpression);
            if (leftType != StdTypesNames.Bool || rightType != StdTypesNames.Bool)
                _errorsHandler.HandleError($"Can not apply operator 'or' to operands of type '{leftType}' and '{rightType}'");
            return StdTypesNames.Bool;
        }

        public string VisitAndExpression(AndExpression andExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, andExpression);
            if (leftType != StdTypesNames.Bool || rightType != StdTypesNames.Bool)
                _errorsHandler.HandleError($"Can not apply operator 'and' to operands of type '{leftType}' and '{rightType}'");
            return StdTypesNames.Bool;
        }

        public string VisitRelativeExpression(RelativeExpression relativeExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, relativeExpression);
            if (leftType != rightType 
                || (leftType != StdTypesNames.Bool && leftType != StdTypesNames.Int) 
                || (leftType == StdTypesNames.Bool && (relativeExpression.Type != RelativeExpressionType.Equal && relativeExpression.Type != RelativeExpressionType.NotEqual)))
            {
                _errorsHandler.HandleError($"Can not apply operator '{RelativeExpressionTypeToOperator.Map[relativeExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            }

            return StdTypesNames.Bool;
        }

        public string VisitAdditiveExpression(AdditiveExpression additiveExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, additiveExpression);
            if (leftType != StdTypesNames.Int || rightType != StdTypesNames.Int)
                _errorsHandler.HandleError($"Can not apply operator '{AdditiveExpressionTypeToOperator.Map[additiveExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            return StdTypesNames.Int;
        }

        public string VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression, ScopeContext scopeContext)
        {
            var (leftType, rightType) = ValidateOperatorExpressions(scopeContext, multiplicativeExpression);
            if (leftType != StdTypesNames.Int || rightType != StdTypesNames.Int)
                _errorsHandler.HandleError($"Can not apply operator '{MultiplicativeExpressionTypeToOperator.Map[multiplicativeExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            return StdTypesNames.Int;
        }

        public string VisitNotExpression(NotExpression notExpression, ScopeContext scopeContext)
        {
            var negatedType = notExpression.Left.AcceptSemanticValidator(this, scopeContext);
            if (negatedType!= StdTypesNames.Bool)
                _errorsHandler.HandleError($"Can not apply operator 'not' to operand of type '{negatedType}'");
            return StdTypesNames.Bool;
        }

        public string VisitVariableExpression(VariableExpression variableExpression, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(variableExpression.Name, out var variable))
                _errorsHandler.HandleError($"Usage of undefined variable '{variableExpression.Name}'");

            if (variable == null)
                return string.Empty;

            if (!variable.IsInitialized)
                _errorsHandler.HandleError($"Usage of uninitialized variable '{variableExpression.Name}'");

            return variable.Type;
        }

        public string VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(propertyCallExpression.ObjectName, out var objectVariable))
                _errorsHandler.HandleError($"Use of undefined variable '{propertyCallExpression.ObjectName}'");

            if (objectVariable == null)
                return string.Empty;

            if (!_definedClasses.ContainsKey(objectVariable.Type))
            {
                _errorsHandler.HandleError($"Variable '{objectVariable.Name}' is not an object and does not support '.' operator");
                return string.Empty;
            }

            if (!objectVariable.IsInitialized)
                _errorsHandler.HandleError($"Variable '{objectVariable.Name}' is not initialized");

            var variableClass = _definedClasses[objectVariable.Type];
            if (!variableClass.DefinedProperties.TryGetValue(propertyCallExpression.PropertyName, out var propertyDeclaration))
                _errorsHandler.HandleError($"Can not resolve property '{propertyCallExpression.PropertyName}' for variable '{objectVariable.Name}' with type '{objectVariable.Type}");

            return propertyDeclaration == null ? string.Empty : propertyDeclaration.Type;
        }

        public string VisitMethodCallExpression(MethodCall methodCall, ScopeContext scopeContext)
        {
            ValidateMethodCall(methodCall, scopeContext);
            var variableType = scopeContext.DefinedVariables[methodCall.ObjectName].Type;
            return _definedClasses[variableType].DefinedMethods[methodCall.Function.Name].Type;
        }

        public string VisitFunctionCallExpression(FunctionCall functionCall, ScopeContext scopeContext)
        {
            ValidateFunctionCall(functionCall, scopeContext);
            return _definedClasses.TryGetValue(functionCall.Name, out var functionClass) 
                ? functionClass.Name 
                : scopeContext.DefinedFunctions[functionCall.Name].Type;
        }

        public string VisitIntLiteralExpression(IntLiteral intLiteral, ScopeContext scopeContext)
        {
            return StdTypesNames.Int;
        }

        public string VisitBoolLiteralExpression(BoolLiteral boolLiteral, ScopeContext scopeContext)
        {
            return StdTypesNames.Bool;
        }

        public string VisitStringLiteralExpression(StringLiteral stringLiteral, ScopeContext scopeContext)
        {
            return StdTypesNames.String;
        }

        private bool ValidateFunctionCall(FunctionCall functionCall, ScopeContext scopeContext)
        {
            if (!_definedClasses.ContainsKey(functionCall.Name) && !scopeContext.DefinedFunctions.ContainsKey(functionCall.Name) && !_stdFunctions.ContainsKey(functionCall.Name))
            {
                _errorsHandler.HandleError($"Call undefined function '{functionCall.Name}'");
                return false;
            }

            FunctionDefinition calledFunction = null;
            if (_definedClasses.ContainsKey(functionCall.Name))
                calledFunction = _definedClasses[functionCall.Name].Constructor;
            else if (scopeContext.DefinedFunctions.ContainsKey(functionCall.Name))
                calledFunction = scopeContext.DefinedFunctions[functionCall.Name];
            else if (_stdFunctions.ContainsKey(functionCall.Name)) calledFunction = _stdFunctions[functionCall.Name];
            if (calledFunction == null)
                throw new Exception("Unexpected error");
            ValidateCallArguments(scopeContext, functionCall, calledFunction);
            return true;
        }

        private bool ValidateMethodCall(MethodCall methodCall, ScopeContext scopeContext)
        {
            if (!scopeContext.DefinedVariables.TryGetValue(methodCall.ObjectName, out var objectVariable))
            {
                _errorsHandler.HandleError($"Use of undefined variable '{methodCall.ObjectName}'");
                return false;
            }

            if (!_definedClasses.TryGetValue(objectVariable.Type, out var objectClass))
            {
                _errorsHandler.HandleError($"Variable '{objectVariable.Name}' is not an object and does not support '.' operator");
                return false;
            }
            
            if (!objectVariable.IsInitialized)
                _errorsHandler.HandleError($"Variable '{objectVariable.Name}' is not initialized");

            if (!objectClass.DefinedMethods.ContainsKey(methodCall.Function.Name))
            {
                _errorsHandler.HandleError($"Call undefined method '{methodCall.Function.Name}' on object '{objectVariable.Name}'");
                return false;
            }

            ValidateCallArguments(scopeContext, methodCall.Function, _definedClasses[objectVariable.Type].DefinedMethods[methodCall.Function.Name], true);
            return true;
        }
        
        private void ValidateCallArguments(ScopeContext scopeContext, FunctionCall functionCall, FunctionDefinition functionDefinition, bool isMethod = false)
        {
            var functionParameters = functionDefinition.Parameters.ToList();
            var functionArguments = functionCall.Arguments.ToList();

            var callType = isMethod ? "Method" : "Function";
            if (functionParameters.Count != functionArguments.Count)
                _errorsHandler.HandleError($"{callType} '{functionDefinition.Name}' has {functionParameters.Count} parameter(s) but is invoked with {functionArguments.Count} argument(s)");

            callType = isMethod ? "method" : "function";
            var length = Math.Min(functionParameters.Count, functionArguments.Count);
            for (var i = 0; i < length; i++)
            {
                var parameterType = functionParameters[i].Type;
                var argumentType = functionArguments[i].AcceptSemanticValidator(this, scopeContext);
                if (parameterType != argumentType)
                    _errorsHandler.HandleError($"Argument type '{argumentType}' is not assignable to parameter type '{parameterType}' in {callType} '{functionCall.Name}' call");
            }
        }

        private (string leftExpressionType, string rightExpressionType) ValidateOperatorExpressions(ScopeContext scopeContext, IOperatorExpression expression)
        {
            return (expression.Left.AcceptSemanticValidator(this, scopeContext), expression.Right.AcceptSemanticValidator(this, scopeContext));
        }

        private void ValidateClassDefinition(DefinedClass definedClass)
        {
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