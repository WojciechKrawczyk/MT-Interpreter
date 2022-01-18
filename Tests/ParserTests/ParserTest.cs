using System.Linq;
using Interpreter.Lexers;
using Interpreter.ParserModule;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.ParserModule.Structures.Expressions.Literals;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.SourceCodeReader;
using Xunit;

namespace Tests.ParserTests
{
    public class ParserTest
    {
        [Theory]
        [InlineData("program { def void Function(bool a) { } }")]
        public void BaseFunctionDefinitionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);
            var function = program.Functions.ToList().Single().Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function", function.Name);
            Assert.Single(function.Parameters);
            var parameter = function.Parameters.Single();
            Assert.Equal("a", parameter.Name);
            Assert.Equal("bool", parameter.Type);
        }

        [Theory]
        [InlineData("program { class Rectangle { def init(){} } }")]
        public void BaseClassDefinitionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Functions);
            Assert.Single(program.Classes);
            var classDefinition = program.Classes.ToList().Single().Value;
            Assert.Equal("Rectangle", classDefinition.Name);
            Assert.NotNull(classDefinition.Constructor);
        }

        [Theory]
        [InlineData("program { def void Function() { int a = 3; bool b; C c; } }")]
        public void VarDeclarationInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);

            var function = program.Functions.ToList().Single().Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Equal(3, function.Instructions.Count());

            var instruction1 = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(VarDeclaration), instruction1.GetType());
            var varDeclaration1 = (VarDeclaration) instruction1;
            Assert.Equal("int", varDeclaration1.Type);
            Assert.Equal("a", varDeclaration1.Name);
            Assert.Equal(typeof(IntLiteral), varDeclaration1.Value.GetType());

            var instruction2 = function.Instructions.ElementAt(1);
            Assert.Equal(typeof(VarDeclaration), instruction2.GetType());
            var varDeclaration2 = (VarDeclaration) instruction2;
            Assert.Equal("bool", varDeclaration2.Type);
            Assert.Equal("b", varDeclaration2.Name);
            Assert.Null(varDeclaration2.Value);

            var instruction3 = function.Instructions.ElementAt(2);
            Assert.Equal(typeof(VarDeclaration), instruction3.GetType());
            var varDeclaration3 = (VarDeclaration) instruction3;
            Assert.Equal("C", varDeclaration3.Type);
            Assert.Equal("c", varDeclaration3.Name);
            Assert.Null(varDeclaration3.Value);
        }

        [Theory]
        [InlineData("program { def void Function(bool a, int b) { a = b < 3; } }")]
        public void AssignmentInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);
            var function = program.Functions.ToList().Single().Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function", function.Name);
            Assert.Equal(2, function.Parameters.Count());
            var parameterA = function.Parameters.ElementAt(0);
            Assert.Equal("a", parameterA.Name);
            Assert.Equal("bool", parameterA.Type);
            var parameterB = function.Parameters.ElementAt(1);
            Assert.Equal("b", parameterB.Name);
            Assert.Equal("int", parameterB.Type);
            Assert.Single(function.Instructions);
            var instruction = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(Assignment), instruction.GetType());
            var assignment = (Assignment) instruction;
            Assert.Equal("a", assignment.VariableName);
            Assert.Equal(typeof(RelativeExpression), assignment.Expression.GetType());
        }

        [Theory]
        [InlineData("program { def void Function1() { Function2(1); } }")]
        public void FunctionCallInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);

            var function = program.Functions.ElementAt(0).Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function1", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Single(function.Instructions);

            var instruction = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(FunctionCall), instruction.GetType());
            var functionCall = (FunctionCall) instruction;
            Assert.Equal("Function2", functionCall.Name);
            Assert.Single(functionCall.Arguments);
            var argument = functionCall.Arguments.ElementAt(0);
            Assert.Equal(typeof(IntLiteral), argument.GetType());
        }

        [Theory]
        [InlineData("program { def void Function1() { a.Function2(1); } }")]
        public void MethodCallInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);

            var function = program.Functions.ElementAt(0).Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function1", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Single(function.Instructions);

            var instruction = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(MethodCall), instruction.GetType());
            var methodCall = (MethodCall) instruction;
            Assert.Equal("a", methodCall.ObjectName);
            Assert.Equal("Function2", methodCall.Function.Name);
            Assert.Single(methodCall.Function.Arguments);
            var argument = methodCall.Function.Arguments.ElementAt(0);
            Assert.Equal(typeof(IntLiteral), argument.GetType());
        }

        [Theory]
        [InlineData("program { def void Function1() { if (1 < 3) { int a = 1; } else { bool b = false; } } }")]
        public void IfInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);

            var function = program.Functions.ElementAt(0).Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function1", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Single(function.Instructions);

            var instruction = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(IfInstruction), instruction.GetType());
            var ifInstruction = (IfInstruction) instruction;
            Assert.Equal(typeof(RelativeExpression), ifInstruction.Condition.GetType());
            Assert.Single(ifInstruction.BaseInstructions);

            var instruction1 = ifInstruction.BaseInstructions.ElementAt(0);
            Assert.Equal(typeof(VarDeclaration), instruction1.GetType());
            Assert.Single(ifInstruction.ElseInstructions);
            var instruction2 = ifInstruction.ElseInstructions.ElementAt(0);
            Assert.Equal(typeof(VarDeclaration), instruction2.GetType());
        }

        [Theory]
        [InlineData("program { def void Function1() { while (a < b) { int a = 1; } } }")]
        public void WhileInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);

            var function = program.Functions.ElementAt(0).Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function1", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Single(function.Instructions);

            var instruction = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(WhileInstruction), instruction.GetType());
            var whileInstruction = (WhileInstruction) instruction;
            Assert.Equal(typeof(RelativeExpression), whileInstruction.Condition.GetType());
            Assert.Single(whileInstruction.Instructions);

            var instruction1 = whileInstruction.Instructions.ElementAt(0);
            Assert.Equal(typeof(VarDeclaration), instruction1.GetType());
        }

        [Theory]
        [InlineData("program { def void Function1() { return; return a; } }")]
        public void ReturnInstructionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Classes);
            Assert.Single(program.Functions);

            var function = program.Functions.ElementAt(0).Value;
            Assert.Equal("void", function.Type);
            Assert.Equal("Function1", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Equal(2, function.Instructions.Count());

            var instruction1 = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(ReturnInstruction), instruction1.GetType());
            var returnInstruction1 = (ReturnInstruction) instruction1;
            Assert.Null(returnInstruction1.ToReturn);

            var instruction2 = function.Instructions.ElementAt(1);
            Assert.Equal(typeof(ReturnInstruction), instruction2.GetType());
            var returnInstruction2 = (ReturnInstruction) instruction2;
            Assert.NotNull(returnInstruction2.ToReturn);
            Assert.Equal(typeof(VariableExpression), returnInstruction2.ToReturn.GetType());
            var variable = (VariableExpression) returnInstruction2.ToReturn;
            Assert.Equal("a", variable.Name);
        }

        [Theory]
        [InlineData("program { class ClassA { def init(int x, int y) { X = x; Y = y; } int X; int Y = 1; def int Method() { return X * Y; } } }")]
        public void ClaasDefinitionTest(string sourceCode)
        {
            var reader = new StringSourceCodeReader(sourceCode);
            var lexer = new Lexer(reader);
            var parser = new Parser(lexer);
            var success = parser.TryToParseProgram(out var program);
            Assert.True(success);
            Assert.Empty(program.Functions);
            Assert.Single(program.Classes);

            var classDefinition = program.Classes.ElementAt(0).Value;
            Assert.Equal("ClassA", classDefinition.Name);
            Assert.Equal(2, classDefinition.Properties.Count);
            var property1 = classDefinition.Properties.ElementAt(0).Value;
            Assert.Equal("int", property1.Type);
            Assert.Equal("X", property1.Name);
            Assert.Null(property1.Value);
            var property2 = classDefinition.Properties.ElementAt(1).Value;
            Assert.Equal("int", property2.Type);
            Assert.Equal("Y", property2.Name);
            Assert.NotNull(property2.Value);
            Assert.Equal(typeof(IntLiteral), property2.Value.GetType());

            var constructor = classDefinition.Constructor;
            Assert.Equal("ClassA", constructor.Type);
            Assert.Equal("ClassA", constructor.Name);
            Assert.Equal(2, constructor.Parameters.Count());
            var parameter1 = constructor.Parameters.ElementAt(0);
            Assert.Equal("int", parameter1.Type);
            Assert.Equal("x", parameter1.Name);
            var parameter2 = constructor.Parameters.ElementAt(1);
            Assert.Equal("int", parameter2.Type);
            Assert.Equal("y", parameter2.Name);
            Assert.Equal(2, constructor.Instructions.Count());
            var instruction1 = constructor.Instructions.ElementAt(0);
            Assert.Equal(typeof(Assignment), instruction1.GetType());
            var assignment1 = (Assignment) instruction1;
            Assert.Equal("X", assignment1.VariableName);
            Assert.Equal(typeof(VariableExpression), assignment1.Expression.GetType());
            var variable1 = (VariableExpression) assignment1.Expression;
            Assert.Equal("x", variable1.Name);
            var instruction2 = constructor.Instructions.ElementAt(1);
            Assert.Equal(typeof(Assignment), instruction2.GetType());
            var assignment2 = (Assignment) instruction2;
            Assert.Equal("Y", assignment2.VariableName);
            Assert.Equal(typeof(VariableExpression), assignment2.Expression.GetType());
            var variable2 = (VariableExpression) assignment2.Expression;
            Assert.Equal("y", variable2.Name);
            
            Assert.Single(classDefinition.Functions);
            var function = classDefinition.Functions.ElementAt(0).Value;
            Assert.Equal("int", function.Type);
            Assert.Equal("Method", function.Name);
            Assert.Empty(function.Parameters);
            Assert.Single(function.Instructions);
            var instruction = function.Instructions.ElementAt(0);
            Assert.Equal(typeof(ReturnInstruction), instruction.GetType());
            var returnInstruction = (ReturnInstruction) instruction;
            Assert.NotNull(returnInstruction.ToReturn);
            Assert.Equal(typeof(MultiplicativeExpression), returnInstruction.ToReturn.GetType());
        }
    }
}