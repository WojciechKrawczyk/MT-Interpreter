using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Errors;
using Interpreter.ParserModule;
using Interpreter.ParserModule.Structures;
using Interpreter.ParserModule.Structures.Definitions;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.ParserModule.Structures.Expressions.Literals;
using Interpreter.ParserModule.Structures.Expressions.Types;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SemanticValidator.Maps;

namespace Interpreter.SemanticValidator
{
    public class SemanticValidator
    {
        private static class BuiltTypesNames
        {
            public const string Int = "int";
            public const string Bool = "bool";
            public const string Void = "void";
        }

        private static readonly List<string> BuiltTypes = new(){"int", "bool"};
        private const string MainFunctionName = "Main";
        private ProgramInstance _programInstance;
        private Dictionary<string, DefinedClass> _definedClasses;
        private Dictionary<string, DefinedFunction> _definedFunctions;
        private Dictionary<string, DefinedFunction> _stdFunctions = new();
        private string _currentFunctionType;

        public void ValidateProgramInstance(ProgramInstance programInstance)
        {
            _programInstance = programInstance;
            _definedClasses = new Dictionary<string, DefinedClass>();
            _definedFunctions = new Dictionary<string, DefinedFunction>();

            ValidateClassDefinitions();
            ValidateFunctionDefinitions();
            ValidateMainFunction();
            foreach (var definedClass in _definedClasses.Values)
            {
                ValidateClassBody(definedClass);
            }
            foreach (var definedFunction in _definedFunctions.Values)
            {
                _currentFunctionType = definedFunction.Type;
                ValidateInstructions(definedFunction.ScopeContext, definedFunction.Instructions);
            }
        }

        private void ValidateClassDefinitions()
        {
            foreach (var classDefinition in _programInstance.Classes.Values)
            {
                if (_definedClasses.ContainsKey(classDefinition.Name))
                {
                    ErrorsHandler.HandleError($"ERROR: Redefinition of class '{classDefinition.Name}'");
                }
                if (classDefinition.Name == MainFunctionName)
                {
                    ErrorsHandler.HandleError($"ERROR: Class can not have name '{MainFunctionName}'");
                }
                _definedClasses.Add(classDefinition.Name, new DefinedClass(classDefinition, new ScopeContext()));
            }
        }

        private void ValidateFunctionDefinitions()
        {
            foreach (var functionDefinition in _programInstance.Functions.Values)
            {
                if (!BuiltTypes.Exists(x => x == functionDefinition.Type) && !_definedClasses.ContainsKey(functionDefinition.Type) && functionDefinition.Type != "void")
                {
                    ErrorsHandler.HandleError($"ERROR: Unknown type '{functionDefinition.Type}' in function {functionDefinition.Name} definition");
                }
                if (_definedClasses.ContainsKey(functionDefinition.Name))
                {
                    ErrorsHandler.HandleError($"ERROR: Function '{functionDefinition.Name}' conflicts with already defined class '{functionDefinition.Name}'");
                }
                if (_definedFunctions.ContainsKey(functionDefinition.Name))
                {
                    ErrorsHandler.HandleError($"ERROR: Redefinition of function '{functionDefinition.Name}'");
                }
                if (_stdFunctions.ContainsKey(functionDefinition.Name))
                {
                    ErrorsHandler.HandleError($"ERROR: Redefinition of standard function '{functionDefinition.Name}'");
                }
                var scopeContext = new ScopeContext();
                foreach (var parameter in functionDefinition.Parameters)
                {
                    if (!BuiltTypes.Exists(x => x == parameter.Type) && !_definedClasses.ContainsKey(parameter.Type))
                    {
                        ErrorsHandler.HandleError($"ERROR: Unknown type '{parameter.Type}' for '{parameter.Name}' argument in function {functionDefinition.Name} definition");
                    }
                    if (scopeContext.HasVariable(parameter.Name))
                    {
                        ErrorsHandler.HandleError($"ERROR: Redefinition of parameter '{parameter.Name} in function '{functionDefinition.Name}' definition");
                    }
                    scopeContext.TryAddVariable(new DefinedVariable(parameter.Type, parameter.Name, true));
                }
                _definedFunctions.Add(functionDefinition.Name, new DefinedFunction(functionDefinition, scopeContext));
            }
        }

        private void ValidateMainFunction()
        {
            if (!_definedFunctions.ContainsKey("Main"))
            {
                ErrorsHandler.HandleError($"ERROR: No 'Main' function is defined");
            }
            if (_definedFunctions.First(x => x.Key == "Main").Value.Parameters.Any())
            {
                ErrorsHandler.HandleError($"ERROR: Function 'Main' can not have parameters");
            }
        }

        private void ValidateClassBody(ClassDefinition classDefinition)
        {
            
        }

        private void ValidateInstructions(ScopeContext scopeContext, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                var instructionType = instruction.GetType();
                if (instructionType == typeof(VarDeclaration))
                {
                    var varDeclaration = (VarDeclaration) instruction;
                    ValidateVarDeclaration(scopeContext, varDeclaration);
                    scopeContext.AddVariable(new DefinedVariable(varDeclaration.Type, varDeclaration.Name, varDeclaration.Value != null));
                }
                else if (instructionType == typeof(Assignment))
                {
                    var assignment = (Assignment) instruction;
                    ValidateAssignment(scopeContext, assignment);
                    scopeContext.SetVariableInitialized(assignment.Variable.Name);
                }
                else if (instructionType == typeof(FunctionCall))
                {
                    var functionCall = (FunctionCall) instruction;
                    ValidateFunctionCall(scopeContext, functionCall);
                    var functionType = _definedFunctions[functionCall.Name].Type;
                    if (functionType != BuiltTypesNames.Void)
                    {
                        ErrorsHandler.HandleWarning($"WARNING: Return value from function '{functionCall.Name}' is not used");
                    }
                }
                else if (instructionType == typeof(MethodCall))
                {
                    var methodCall = (MethodCall) instruction;
                    ValidateMethodCall(scopeContext, methodCall);
                    scopeContext.TryGetVariable(methodCall.ObjectName, out var variable);
                    var methodType = _definedClasses[variable.Type].Functions[methodCall.Function.Name].Type;
                    if (methodType != BuiltTypesNames.Void)
                    {
                        ErrorsHandler.HandleWarning($"WARNING: Return value from method '{methodCall.Function.Name}' call on object '{methodCall.ObjectName}' is not used");
                    }
                }
                else if (instructionType == typeof(IfInstruction))
                {
                    var ifInstruction = (IfInstruction) instruction;
                    ValidateIfInstruction(scopeContext, ifInstruction);
                }
                else if (instructionType == typeof(WhileInstruction))
                {
                    var whileInstruction = (WhileInstruction) instruction;
                    ValidateWhileInstruction(scopeContext, whileInstruction);
                }
                else if (instructionType == typeof(ReturnInstruction))
                {
                    var returnInstruction = (ReturnInstruction) instruction;
                    ValidateReturnInstruction(scopeContext, returnInstruction);
                }
            }
        }

        private void ValidateVarDeclaration(ScopeContext scopeContext, VarDeclaration varDeclaration)
        {
            if (!BuiltTypes.Exists(x => x == varDeclaration.Type) && !_definedClasses.ContainsKey(varDeclaration.Name))
            {
                ErrorsHandler.HandleError($"ERROR: Declaration variable '{varDeclaration.Name}' of unknown type '{varDeclaration.Type}'");
            }
            if (!scopeContext.TryAddVariable(new DefinedVariable(varDeclaration.Type, varDeclaration.Name, varDeclaration.Value != null)))
            {
                ErrorsHandler.HandleError($"ERROR: Redeclaration of variable '{varDeclaration.Name}'");
            }
            if (varDeclaration.Value == null)
            {
                return;
            }
            ValidateExpression(scopeContext, varDeclaration.Value);
            var expressionType = DetermineExpressionType(scopeContext, varDeclaration.Value);
            if (varDeclaration.Type != expressionType)
            {
                ErrorsHandler.HandleError($"ERROR: Unable assign expression with type '{expressionType}' to variable '{varDeclaration.Name}' with type '{varDeclaration.Type}'");
            }
        }

        private void ValidateAssignment(ScopeContext scopeContext, Assignment assignment)
        {
            if (!scopeContext.TryGetVariable(assignment.Variable.Name, out var variable))
            {
                ErrorsHandler.HandleError($"ERROR: Assignment to undefined variable '{variable.Name}'");
            }
            var expressionType = DetermineExpressionType(scopeContext, assignment.Expression);
            if (variable.Type != expressionType)
            {
                ErrorsHandler.HandleError($"ERROR: Unable assign expression with type '{expressionType}' to variable '{variable.Name}' with type '{variable.Type}'");
            }
        }

        private void ValidateIfInstruction(ScopeContext scopeContext, IfInstruction ifInstruction)
        {
            ValidateExpression(scopeContext, ifInstruction.Condition);
            if (DetermineExpressionType(scopeContext, ifInstruction.Condition) != BuiltTypesNames.Bool)
            {
                ErrorsHandler.HandleError($"ERROR: Condition expression has to have '{BuiltTypesNames.Bool}' type");
            }
            ValidateInstructions(scopeContext, ifInstruction.BaseInstructions);
            ValidateInstructions(scopeContext, ifInstruction.BaseInstructions);
        }

        private void ValidateWhileInstruction(ScopeContext scopeContext, WhileInstruction whileInstruction)
        {
            ValidateExpression(scopeContext, whileInstruction.Condition);
            if (DetermineExpressionType(scopeContext, whileInstruction.Condition) != BuiltTypesNames.Bool)
            {
                ErrorsHandler.HandleError($"ERROR: Condition expression has to have '{BuiltTypesNames.Bool}' type");
            }
            ValidateInstructions(scopeContext, whileInstruction.Instructions);
        }

        private void ValidateReturnInstruction(ScopeContext scopeContext, ReturnInstruction returnInstruction)
        {
            ValidateExpression(scopeContext, returnInstruction.ToReturn);
            var expressionType = DetermineExpressionType(scopeContext, returnInstruction.ToReturn);
            if (expressionType != _currentFunctionType)
            {
                ErrorsHandler.HandleError($"ERROR: Can not convert expression type '{expressionType}' to return type '{_currentFunctionType}'");
            }
        }

        private string DetermineExpressionType(ScopeContext scopeContext, IExpression expression)
        {
            var expressionType = expression.GetType();
            if (expressionType == typeof(FunctionCall))
            {
                var functionCall = (FunctionCall) expression;
                return _definedFunctions[functionCall.Name].Type;
            }
            if (expressionType == typeof(MethodCall))
            {
                var methodCall = (MethodCall) expression;
                scopeContext.TryGetVariable(methodCall.ObjectName, out var variable);
                return _definedClasses[variable.Type].Functions[methodCall.Function.Name].Type;
            }
            if (expressionType == typeof(PropertyCall))
            {
                var propertyCall = (PropertyCall) expression;
                scopeContext.TryGetVariable(propertyCall.ObjectName, out var variable);
                return _definedClasses[variable.Type].Properties[propertyCall.PropertyName].Type;
            }
            if (expressionType == typeof(Variable))
            {
                var variableCall = (Variable) expression;
                scopeContext.TryGetVariable(variableCall.Name, out var variable);
                return variable.Type;
            }
            if (expressionType == typeof(MultiplicativeExpression) || expressionType == typeof(AdditiveExpression) || expressionType == typeof(IntLiteral))
            {
                return BuiltTypesNames.Int;
            }
            if (expressionType == typeof(AndExpression) || expressionType == typeof(OrExpression) || expressionType == typeof(NotExpression) || expressionType == typeof(RelativeExpression) || expressionType == typeof(BoolLiteral))
            {
                return BuiltTypesNames.Bool;
            }
            throw new Exception("Unable to determine expression type");
        }

        private void ValidateExpression(ScopeContext scopeContext, IExpression expression)
        {
            var expressionType = expression.GetType();
            if (expressionType == typeof(FunctionCall))
                ValidateFunctionCall(scopeContext, (FunctionCall) expression);
            else if (expressionType == typeof(MethodCall))
                ValidateMethodCall(scopeContext, (MethodCall) expression);
            else if (expressionType == typeof(PropertyCall))
                ValidatePropertyCall(scopeContext, (PropertyCall) expression);
            else if (expressionType == typeof(Variable))
                ValidateVariable(scopeContext, (Variable) expression);
            else if (expressionType == typeof(MultiplicativeExpression))
                ValidateMultiplicativeExpression(scopeContext, (MultiplicativeExpression) expression);
            else if (expressionType == typeof(AdditiveExpression))
                ValidateAdditiveExpression(scopeContext, (AdditiveExpression) expression);
            else if (expressionType == typeof(RelativeExpression))
                ValidateRelativeExpression(scopeContext, (RelativeExpression) expression);
            else if (expressionType == typeof(AndExpression))
                ValidateAndExpression(scopeContext, (AndExpression) expression);
            else if (expressionType == typeof(OrExpression))
                ValidateOrExpression(scopeContext, (OrExpression) expression);
            else if (expressionType == typeof(NotExpression))
                ValidateNotExpression(scopeContext, (NotExpression) expression);
        }

        private void ValidateFunctionCall(ScopeContext scopeContext, FunctionCall functionCall)
        {
            if (!_definedClasses.ContainsKey(functionCall.Name) && !_definedFunctions.ContainsKey(functionCall.Name) && !_stdFunctions.ContainsKey(functionCall.Name))
            {
                ErrorsHandler.HandleError($"ERROR: Call undefined function '{functionCall.Name}'");
            }

            FunctionDefinition calledFunction = null;
            if (_definedClasses.ContainsKey(functionCall.Name))
            {
                calledFunction = _definedClasses[functionCall.Name].Constructor;
            }
            else if (_definedFunctions.ContainsKey(functionCall.Name))
            {
                calledFunction = _definedFunctions[functionCall.Name];
            }
            else if (_stdFunctions.ContainsKey(functionCall.Name))
            {
                calledFunction = _stdFunctions[functionCall.Name];
            }
            if (calledFunction == null)
            {
                throw new Exception();
            }
            ValidateCallArguments(scopeContext, functionCall, _definedFunctions[functionCall.Name]);
        }

        private void ValidateMethodCall(ScopeContext scopeContext, MethodCall methodCall)
        {
            if (!scopeContext.TryGetVariable(methodCall.ObjectName, out var objectVariable))
            {
                ErrorsHandler.HandleError($"ERROR: Use of undefined variable {methodCall.ObjectName}");
            }
            if (BuiltTypes.Contains(objectVariable.Type))
            {
                ErrorsHandler.HandleError($"ERROR: Variable '{objectVariable.Name}' has no complex type");
            }
            var variableClass = _definedClasses[objectVariable.Type];
            if (!variableClass.Functions.ContainsKey(methodCall.Function.Name))
            {
                ErrorsHandler.HandleError($"ERROR: Variable '{objectVariable.Name}' has type '{objectVariable.Type}' and has not method '{methodCall.Function.Name}'");
            }
            ValidateCallArguments(scopeContext, methodCall.Function, _definedFunctions[methodCall.Function.Name], true);
        }

        private void ValidateCallArguments(ScopeContext scopeContext, FunctionCall functionCall, FunctionDefinition functionDefinition, bool isMethod = false)
        {
            if (functionDefinition.Parameters.Count() != functionCall.Arguments.Count())
            {
                ErrorsHandler.HandleError($"ERROR: Function '{functionDefinition.Name} has {functionDefinition.Parameters.Count()} parameter(s) but is invoked with {functionCall.Arguments.Count()} argument(s)");
            }

            var callType = isMethod ? "method" : "function";
            var functionParameters = functionDefinition.Parameters.ToList();
            var functionArguments = functionCall.Arguments.ToList();
            for (var i = 0; i < functionArguments.Count; i++)
            {
                var parameter = functionParameters[i];
                var argument = functionArguments[i];
                ValidateExpression(scopeContext, argument);
                var argumentType = DetermineExpressionType(scopeContext, functionArguments[i]);
                if (parameter.Type != argumentType)
                {
                    ErrorsHandler.HandleError($"ERROR: Argument type '{argumentType}' is not assignable to parameter type '{parameter.Type}' in {callType} '{functionCall.Name}' call");
                }
            }
        }

        private void ValidatePropertyCall(ScopeContext scopeContext, PropertyCall propertyCall)
        {
            if (!scopeContext.TryGetVariable(propertyCall.ObjectName, out var objectVariable))
            {
                ErrorsHandler.HandleError($"ERROR: Use of undefined variable {propertyCall.ObjectName}");
            }
            if (BuiltTypes.Contains(objectVariable.Type))
            {
                ErrorsHandler.HandleError($"ERROR: Variable '{objectVariable.Name}' has no complex type");
            }
            var variableClass = _definedClasses[objectVariable.Type];
            if (!variableClass.Properties.ContainsKey(propertyCall.PropertyName))
            {
                ErrorsHandler.HandleError($"ERROR: Can not resolve property '{propertyCall.PropertyName}' for variable '{objectVariable.Name}' with type '{objectVariable.Type}");
            }
        }

        private void ValidateVariable(ScopeContext scopeContext, Variable variable)
        {
            if (!scopeContext.TryGetVariable(variable.Name, out var variableFromContext))
            {
                ErrorsHandler.HandleError($"ERROR: Usage of undefined variable '{variable.Name}'");
            }
            if (!variableFromContext.IsInitialized)
            {
                ErrorsHandler.HandleError($"ERROR: Usage of uninitialized variable '{variable.Name}'");
            }
        }

        private void ValidateAdditiveExpression(ScopeContext scopeContext, AdditiveExpression additiveExpression)
        {
            var (leftType rightType) = ValidateAndGetInnerExpressionsTypes(scopeContext, additiveExpression);
            if (leftType != BuiltTypesNames.Int || rightType != BuiltTypesNames.Int)
            {
                ErrorsHandler.HandleError($"Can not apply operator '{AdditiveExpressionTypeToOperator.Map[additiveExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            }
        }

        private void ValidateMultiplicativeExpression(ScopeContext scopeContext, MultiplicativeExpression multiplicativeExpression)
        {
            var (leftType rightType) = ValidateAndGetInnerExpressionsTypes(scopeContext, multiplicativeExpression);
            if (leftType != BuiltTypesNames.Int || rightType != BuiltTypesNames.Int)
            {
                ErrorsHandler.HandleError($"Can not apply operator '{MultiplicativeExpressionTypeToOperator.Map[multiplicativeExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            }
        }

        private void ValidateRelativeExpression(ScopeContext scopeContext, RelativeExpression relativeExpression)
        {
            var (leftType rightType) = ValidateAndGetInnerExpressionsTypes(scopeContext, relativeExpression);
            if (leftType != rightType 
                || (leftType != BuiltTypesNames.Bool && leftType != BuiltTypesNames.Int) 
                || (leftType == BuiltTypesNames.Bool && (relativeExpression.Type != RelativeExpressionType.Equal && relativeExpression.Type != RelativeExpressionType.NotEqual)))
            {
                ErrorsHandler.HandleError($"Can not apply operator '{RelativeExpressionTypeToOperator.Map[relativeExpression.Type]}' to operands of type '{leftType}' and '{rightType}'");
            }
        }

        private void ValidateAndExpression(ScopeContext scopeContext, AndExpression andExpression)
        {
            var (leftType rightType) = ValidateAndGetInnerExpressionsTypes(scopeContext, andExpression);
            if (leftType != BuiltTypesNames.Bool || rightType != BuiltTypesNames.Bool)
            {
                ErrorsHandler.HandleError($"Can not apply operator 'and' to operands of type '{leftType}' and '{rightType}'");
            }
        }

        private void ValidateOrExpression(ScopeContext scopeContext, OrExpression orExpression)
        {
            var (leftType rightType) = ValidateAndGetInnerExpressionsTypes(scopeContext, orExpression);
            if (leftType != BuiltTypesNames.Bool || rightType != BuiltTypesNames.Bool)
            {
                ErrorsHandler.HandleError($"Can not apply operator 'or' to operands of type '{leftType}' and '{rightType}'");
            }
        }

        private void ValidateNotExpression(ScopeContext scopeContext, NotExpression notExpression)
        {
            ValidateExpression(scopeContext, notExpression.Left);
            var type = DetermineExpressionType(scopeContext, notExpression.Left);
            if (type != BuiltTypesNames.Bool)
            {
                ErrorsHandler.HandleError($"Can not apply operator 'not' to operand of type '{type}'");
            }
        }

        private (string leftExpressionType, string rightExpressionType) ValidateAndGetInnerExpressionsTypes(ScopeContext scopeContext, Expression expression)
        {
            ValidateExpression(scopeContext, expression.Left);
            ValidateExpression(scopeContext, expression.Right);
            var leftType = DetermineExpressionType(scopeContext, expression.Left);
            var rightType = DetermineExpressionType(scopeContext, expression.Right);
            return (leftType, rightType);
        }
    }
}