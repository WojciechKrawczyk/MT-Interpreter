using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.ParserModule.Structures.Expressions.Literals;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;
using Interpreter.Modules.ParserModule.Structures.Instructions;
using Interpreter.Modules.SemanticValidatorModule;
using Interpreter.Modules.SemanticValidatorModule.ValidStructures;
using Interpreter.Modules.StdResources;

namespace Interpreter.Executor
{
    public class Executor : IStructuresExecutorVisitor
    {
        private ExecutableVariable _executableVariableToReturn;
        
        public void ExecuteProgram(ValidProgramInstance validProgramInstance)
        {
            var mainScopeContext = new ExecutableScopeContext
            {
                Functions = validProgramInstance.Functions, //dodac te z std
                Classes = validProgramInstance.Classes,
                Variables = new Dictionary<string, ExecutableVariable>()
            };

            var mainFunction = validProgramInstance.Functions[StdNames.MainFunctionName];
            ExecuteFunction(mainFunction, mainScopeContext);
        }

        private ExecutableVariable ExecuteFunction(FunctionDefinition functionDefinition, ExecutableScopeContext executableScopeContext)
        {
            try
            {
                ExecuteInstructionsBlock(functionDefinition.Instructions, executableScopeContext);
            }
            catch (ReturnException returnException)
            {
                return _executableVariableToReturn.Clone();
            }

            throw new Exception();
        }

        private void ExecuteInstructionsBlock(IEnumerable<IInstruction> instructions, ExecutableScopeContext executableScopeContext)
        {
            foreach (var instruction in instructions)
            {
                instruction.AcceptExecutor(this, executableScopeContext);
            }
        }

        public void VisitVarDeclarationInstruction(VarDeclaration varDeclaration, ExecutableScopeContext executableScopeContext)
        {
            var variable = varDeclaration.Value.AcceptExecutor(this, executableScopeContext);
            variable.Type = varDeclaration.Type;
            variable.Name = varDeclaration.Name;
            executableScopeContext.Variables.Add(variable.Name, variable);
        }

        public void VisitAssignmentInstruction(Assignment assignment, ExecutableScopeContext executableScopeContext)
        {
            var newValue = assignment.Expression.AcceptExecutor(this, executableScopeContext);
            executableScopeContext.Variables[assignment.VariableName] = newValue;
        }

        public void VisitFunctionCallInstruction(FunctionCall functionCall, ExecutableScopeContext executableScopeContext)
        {
            var functionContext = ExecuteCallArguments(functionCall, executableScopeContext.Functions[functionCall.Name], executableScopeContext);
            functionContext.Classes = executableScopeContext.Classes;
            functionContext.Functions = executableScopeContext.Functions;
            ExecuteFunction(executableScopeContext.Functions[functionCall.Name], functionContext);
        }

        public void VisitMethodCallInstruction(MethodCall methodCall, ExecutableScopeContext executableScopeContext)
        {
            var method = executableScopeContext.Classes[executableScopeContext.Variables[methodCall.ObjectName].Type].Methods[methodCall.Function.Name];
            var methodContext = ExecuteCallArguments(methodCall.Function, method, executableScopeContext);
            methodContext.Classes = executableScopeContext.Classes;
            methodContext.Functions = executableScopeContext.Classes[executableScopeContext.Variables[methodCall.ObjectName].Type].Methods;//i te z std
            foreach (var (key, value) in executableScopeContext.Variables[methodCall.ObjectName].Properties)
            {
                methodContext.Variables.Add(key, value);
            }
            ExecuteFunction(method, methodContext);
        }

        private ExecutableScopeContext ExecuteCallArguments(FunctionCall functionCall, FunctionDefinition functionDefinition, ExecutableScopeContext executableScopeContext)
        {
            var functionParameters = functionDefinition.Parameters.ToList();
            var functionArguments = functionCall.Arguments.ToList();

            var newContext = new ExecutableScopeContext();
            for (var i = 0; i < functionArguments.Count; i++)
            {
                var argumentValue = functionArguments[i].AcceptExecutor(this, executableScopeContext);
                var parameter = functionParameters[i];
                argumentValue.Type = parameter.Type;
                argumentValue.Name = parameter.Name;
                newContext.Variables.Add(argumentValue.Name, argumentValue);
            }

            return newContext;
        }

        public void VisitReturnInstruction(ReturnInstruction returnInstruction, ExecutableScopeContext executableScopeContext)
        {
            if (returnInstruction.ToReturn != null)
            {
                _executableVariableToReturn = returnInstruction.ToReturn.AcceptExecutor(this, executableScopeContext);
                throw new ReturnException();
            }
            _executableVariableToReturn = null;
            throw new ReturnException();
        }

        public void VisitIfInstruction(IfInstruction ifInstruction, ExecutableScopeContext executableScopeContext)
        {
            var baseVariablesNames = executableScopeContext.Variables.Keys.ToList();
            var conditionValue = ifInstruction.Condition.AcceptExecutor(this, executableScopeContext);
            var actualValue = bool.Parse(conditionValue.Value);
            if (actualValue)
            {
                ExecuteInstructionsBlock(ifInstruction.BaseInstructions, executableScopeContext);
            }
            else if (ifInstruction.ElseInstructions != null)
            {
                ExecuteInstructionsBlock(ifInstruction.ElseInstructions, executableScopeContext);
            }

            var variablesNamesToDelete = executableScopeContext.Variables.Keys.Where(x => !baseVariablesNames.Contains(x)).ToList();
            foreach (var variableNameToDelete in variablesNamesToDelete)
            {
                executableScopeContext.Variables.Remove(variableNameToDelete);
            }
        }

        public void VisitWhileInstruction(WhileInstruction whileInstruction, ExecutableScopeContext executableScopeContext)
        {
            var baseVariablesNames = executableScopeContext.Variables.Keys.ToList();
            var conditionValue = whileInstruction.Condition.AcceptExecutor(this, executableScopeContext);
            var actualValue = bool.Parse(conditionValue.Value);
            while (actualValue)
            {
                ExecuteInstructionsBlock(whileInstruction.Instructions, executableScopeContext);
                conditionValue = whileInstruction.Condition.AcceptExecutor(this, executableScopeContext);
                actualValue = bool.Parse(conditionValue.Value);
            }

            var variablesNamesToDelete = executableScopeContext.Variables.Keys.Where(x => !baseVariablesNames.Contains(x)).ToList();
            foreach (var variableNameToDelete in variablesNamesToDelete)
            {
                executableScopeContext.Variables.Remove(variableNameToDelete);
            }
        }

        public ExecutableVariable VisitOrExpression(OrExpression orExpression, ExecutableScopeContext executableScopeContext)
        {
            var leftArgumentValue = orExpression.Left.AcceptExecutor(this, executableScopeContext);
            var actualLeftArgumentValue = bool.Parse(leftArgumentValue.Value);
            if (actualLeftArgumentValue)
            {
                return new ExecutableVariable
                {
                    Type = StdTypesNames.Bool,
                    Value = bool.TrueString
                };
            }

            var rightArgumentValue = orExpression.Right.AcceptExecutor(this, executableScopeContext);
            var actualRightArgumentValue = bool.Parse(rightArgumentValue.Value);
            return new ExecutableVariable
            {
                Type = StdTypesNames.Bool,
                Value = actualRightArgumentValue.ToString()
            };
        }

        public ExecutableVariable VisitAndExpression(AndExpression andExpression, ExecutableScopeContext executableScopeContext)
        {
            var leftArgumentValue = andExpression.Left.AcceptExecutor(this, executableScopeContext);
            var actualLeftArgumentValue = bool.Parse(leftArgumentValue.Value);
            if (!actualLeftArgumentValue)
            {
                return new ExecutableVariable
                {
                    Type = StdTypesNames.Bool,
                    Value = bool.FalseString
                };
            }

            var rightArgumentValue = andExpression.Right.AcceptExecutor(this, executableScopeContext);
            var actualRightArgumentValue = bool.Parse(rightArgumentValue.Value);
            return new ExecutableVariable
            {
                Type = StdTypesNames.Bool,
                Value = actualRightArgumentValue.ToString()
            };
        }

        public ExecutableVariable VisitRelativeExpression(RelativeExpression relativeExpression, ExecutableScopeContext executableScopeContext)
        {
            var leftArgumentValue = relativeExpression.Left.AcceptExecutor(this, executableScopeContext);
            var rightArgumentValue = relativeExpression.Right.AcceptExecutor(this, executableScopeContext);

            bool value;
            if (leftArgumentValue.Type == StdTypesNames.Bool)
            {
                var actualBoolLeftArgumentValue = bool.Parse(leftArgumentValue.Value);
                var actualBoolRightArgumentValue = bool.Parse(rightArgumentValue.Value);
                value = relativeExpression.Type == RelativeExpressionType.Equal
                    ? actualBoolLeftArgumentValue == actualBoolRightArgumentValue
                    : actualBoolLeftArgumentValue != actualBoolRightArgumentValue;
            }
            else
            {
                var actualIntLeftArgumentValue = int.Parse(leftArgumentValue.Value);
                var actualIntRightArgumentValue = int.Parse(rightArgumentValue.Value);
                value = relativeExpression.Type switch
                {
                    RelativeExpressionType.Equal => actualIntLeftArgumentValue == actualIntRightArgumentValue,
                    RelativeExpressionType.NotEqual => actualIntLeftArgumentValue != actualIntRightArgumentValue,
                    RelativeExpressionType.Less => actualIntLeftArgumentValue < actualIntRightArgumentValue,
                    RelativeExpressionType.LessOrEqual => actualIntLeftArgumentValue <= actualIntRightArgumentValue,
                    RelativeExpressionType.Grater => actualIntLeftArgumentValue > actualIntRightArgumentValue,
                    RelativeExpressionType.GraterOrEqual => actualIntLeftArgumentValue >= actualIntRightArgumentValue,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            return new ExecutableVariable
            {
                Type = StdTypesNames.Bool,
                Value = value.ToString()
            };
        }

        public ExecutableVariable VisitAdditiveExpression(AdditiveExpression additiveExpression, ExecutableScopeContext executableScopeContext)
        {
            var leftArgumentValue = additiveExpression.Left.AcceptExecutor(this, executableScopeContext);
            var rightArgumentValue = additiveExpression.Right.AcceptExecutor(this, executableScopeContext);
            var actualIntLeftArgumentValue = int.Parse(leftArgumentValue.Value);
            var actualIntRightArgumentValue = int.Parse(rightArgumentValue.Value);
            var value = additiveExpression.Type switch
            {
                AdditiveExpressionType.Minus => actualIntLeftArgumentValue - actualIntRightArgumentValue,
                AdditiveExpressionType.Plus => actualIntLeftArgumentValue + actualIntRightArgumentValue,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ExecutableVariable
            {
                Type = StdTypesNames.Int,
                Value = value.ToString()
            };
        }

        public ExecutableVariable VisitMultiplicativeExpression(MultiplicativeExpression multiplicativeExpression, ExecutableScopeContext executableScopeContext)
        {
            var leftArgumentValue = multiplicativeExpression.Left.AcceptExecutor(this, executableScopeContext);
            var rightArgumentValue = multiplicativeExpression.Right.AcceptExecutor(this, executableScopeContext);
            var actualIntLeftArgumentValue = int.Parse(leftArgumentValue.Value);
            var actualIntRightArgumentValue = int.Parse(rightArgumentValue.Value);
            var value = multiplicativeExpression.Type switch
            {
                MultiplicativeExpressionType.Division => actualIntLeftArgumentValue / actualIntRightArgumentValue,
                MultiplicativeExpressionType.Multiplication => actualIntLeftArgumentValue * actualIntRightArgumentValue,
                MultiplicativeExpressionType.Modulo => actualIntLeftArgumentValue % actualIntRightArgumentValue,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ExecutableVariable
            {
                Type = StdTypesNames.Int,
                Value = value.ToString()
            };
        }

        public ExecutableVariable VisitNotExpression(NotExpression notExpression, ExecutableScopeContext executableScopeContext)
        {
            var argumentValue = notExpression.Left.AcceptExecutor(this, executableScopeContext);
            var actualArgumentValue = bool.Parse(argumentValue.Value);
            return new ExecutableVariable
            {
                Type = StdTypesNames.Bool,
                Value = (!actualArgumentValue).ToString()
            };
        }

        public ExecutableVariable VisitVariableExpression(VariableExpression variableExpression, ExecutableScopeContext executableScopeContext)
        {
            var variable = executableScopeContext.Variables[variableExpression.Name];
            return variable.Clone();
        }

        public ExecutableVariable VisitPropertyCallExpression(PropertyCallExpression propertyCallExpression, ExecutableScopeContext executableScopeContext)
        {
            var variable = executableScopeContext.Variables[propertyCallExpression.ObjectName].Properties[propertyCallExpression.PropertyName];
            return variable.Clone();
        }

        public ExecutableVariable VisitMethodCallExpression(MethodCall methodCall, ExecutableScopeContext executableScopeContext)
        {
            throw new System.NotImplementedException();
        }

        public ExecutableVariable VisitFunctionCallExpression(FunctionCall functionCall, ExecutableScopeContext executableScopeContext)
        {
            throw new System.NotImplementedException();
        }

        public ExecutableVariable VisitIntLiteralExpression(IntLiteral intLiteral, ExecutableScopeContext executableScopeContext) =>
            new()
            {
                Type = StdTypesNames.Bool,
                Value = intLiteral.Value.ToString()
            };

        public ExecutableVariable VisitBoolLiteralExpression(BoolLiteral boolLiteral, ExecutableScopeContext executableScopeContext) =>
            new()
            {
                Type = StdTypesNames.Bool,
                Value = boolLiteral.Value.ToString()
            };

        public ExecutableVariable VisitStringLiteralExpression(StringLiteral stringLiteral, ExecutableScopeContext executableScopeContext) =>
            new()
            {
                Type = StdTypesNames.Bool,
                Value = stringLiteral.Value
            };
        
        private class ReturnException : Exception
        {
            
        }
    }
}