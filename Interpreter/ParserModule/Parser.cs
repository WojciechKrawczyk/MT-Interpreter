using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Lexers;
using Interpreter.ParserModule.Structures;
using Interpreter.ParserModule.Structures.Definitions;
using Interpreter.ParserModule.Structures.Expressions;
using Interpreter.ParserModule.Structures.Expressions.Literals;
using Interpreter.ParserModule.Structures.Expressions.Types.Maps;
using Interpreter.ParserModule.Structures.Instructions;
using Interpreter.ParserModule.Types;
using Interpreter.Tokens;

namespace Interpreter.ParserModule
{
    public class Parser
    {
        private readonly ILexer _lexer;
        private readonly Dictionary<string, FunctionDefinition> _functions = new();
        private readonly Dictionary<string, ClassDefinition> _classes = new();

        public Parser(ILexer lexer)
        {
            _lexer = lexer;
        }

        public bool TryToParseProgram(out ProgramInstance program)
        {
            MustBe(TokenType.Program);
            MustBe(TokenType.CurlyOpenBracket);
            INode definition;
            while (TryToParseFunctionDefinition(out definition) || TryToParseClassDefinition(out definition))
            {
                HandleDefinition(definition);
            }
            MustBe(TokenType.CurlyCloseBracket);
            program = new ProgramInstance(_functions, _classes);
            return true;
        }

        private void HandleDefinition(INode definition)
        {
            switch (definition)
            {
                case FunctionDefinition functionDefinition
                    when !_functions.TryAdd(functionDefinition.Name, functionDefinition):
                    throw new Exception();
                case ClassDefinition classDefinition
                    when !_classes.TryAdd(classDefinition.Name, classDefinition):
                    throw new Exception();
            }
        }

        private bool TryToParseFunctionDefinition(out INode functionDefinition)
        {
            functionDefinition = null;
            if (!Preview(TokenType.Def))
                return false;

            MustBe(new [] {TokenType.Int, TokenType.Bool, TokenType.Void, TokenType.Identifier});
            var type = TokenToType.Map(_lexer.CurrentToken);

            MustBe(TokenType.Identifier);
            var name = _lexer.CurrentToken.Lexeme;

            MustBe(TokenType.RoundOpenBracket);
            var parameters = ParseParameters();
            MustBe(TokenType.RoundCloseBracket);

            MustBe(TokenType.CurlyOpenBracket);
            var instructions = ParseInstructions();
            MustBe(TokenType.CurlyCloseBracket);

            functionDefinition = new FunctionDefinition(name, type, parameters, instructions);
            return true;
        }

        private IEnumerable<Parameter> ParseParameters()
        {
            var parameters = new List<Parameter>();
            var availableTypes = new[] {TokenType.Int, TokenType.Bool, TokenType.Identifier};
            if (!Preview(availableTypes))
                return parameters;

            var type = TokenToType.Map(_lexer.CurrentToken);
            MustBe(TokenType.Identifier);
            var name = _lexer.CurrentToken.Lexeme;
            parameters.Add(new Parameter(type, name));
            while (Preview(TokenType.Comma))
            {
                MustBe(availableTypes);
                type = TokenToType.Map(_lexer.CurrentToken);
                MustBe(TokenType.Identifier);
                name = _lexer.CurrentToken.Lexeme;
                if (parameters.Any(x => x.Name == name))
                    throw new Exception("");
                parameters.Add(new Parameter(type, name));
            }
            return parameters;
        }

        private IEnumerable<Instruction> ParseInstructions()
        {
            var instructions = new List<Instruction>();
            Instruction instruction;
            while (TryToParseIfInstruction(out instruction)
                   || TryToParseWhileInstruction(out instruction)
                   || TryToParseFlatInitInstruction(out instruction)
                   || TryToParseIdentifierInstruction(out instruction)
                   || TryToParseReturnInstruction(out instruction))
            {
                instructions.Add(instruction);
            }
            return instructions;
        }

        private bool TryToParseIfInstruction(out Instruction instruction)
        {
            instruction = null;
            if (!Preview(TokenType.If))
                return false;

            MustBe(TokenType.RoundOpenBracket);
            if (!TryToParseExpression(out var condition))
                throw new Exception("");
            MustBe(TokenType.RoundCloseBracket);
            
            MustBe(TokenType.CurlyOpenBracket);
            var instructions = ParseInstructions();
            MustBe(TokenType.CurlyCloseBracket);

            IEnumerable<Instruction> elseInstructions = null;
            if (Preview(TokenType.Else))
            {
                MustBe(TokenType.CurlyOpenBracket);
                elseInstructions = ParseInstructions();
                MustBe(TokenType.CurlyCloseBracket);
            }

            instruction = new IfInstruction(condition, instructions, elseInstructions);
            return true;
        }

        private bool TryToParseWhileInstruction(out Instruction instruction)
        {
            instruction = null;
            if (!Preview(TokenType.While))
                return false;

            MustBe(TokenType.RoundOpenBracket);
            if (!TryToParseExpression(out var condition))
                throw new Exception("");
            MustBe(TokenType.RoundCloseBracket);
            
            MustBe(TokenType.CurlyOpenBracket);
            var instructions = ParseInstructions();
            MustBe(TokenType.CurlyCloseBracket);

            instruction = new WhileInstruction(condition, instructions);
            return true;
        }

        private bool TryToParseFlatInitInstruction(out Instruction instruction)
        {
            instruction = null;
            if (!Preview(new[] {TokenType.Int, TokenType.Bool}))
                return false;

            var type = TokenToType.Map(_lexer.CurrentToken);
            MustBe(TokenType.Identifier);
            var name = _lexer.CurrentToken.Lexeme;
            IExpression expression = null;
            if (Preview(TokenType.Assign) && !TryToParseExpression(out expression))
                throw new Exception("");
            MustBe(TokenType.Semicolon);
            
            instruction = new VarDeclaration(name, type, expression);
            return true;
        }

        private bool TryToParseIdentifierInstruction(out Instruction instruction)
        {
            instruction = null;
            if (!Preview(TokenType.Identifier))
                return false;

            var token = _lexer.CurrentToken;

            if (Preview(TokenType.Identifier))
            {
                var name = _lexer.CurrentToken.Lexeme;
                IExpression expression = null;
                if (Preview(TokenType.Assign) && !TryToParseExpression(out expression))
                    throw new Exception("");
                MustBe(TokenType.Semicolon);
                instruction = new VarDeclaration(name, TokenToType.Map(token), expression);
                return true;
            }

            if (Preview(TokenType.Assign))
            {
                if (!TryToParseExpression(out var expression))
                    throw new Exception("");
                MustBe(TokenType.Semicolon);
                instruction = new Assignment(new Variable(token.Lexeme), expression);
                return true;
            }

            if (Preview(TokenType.RoundOpenBracket))
            {
                var arguments = ParseArguments();
                MustBe(TokenType.RoundCloseBracket);
                MustBe(TokenType.Semicolon);
                instruction = new FunctionCall(token.Lexeme, arguments);
                return true;
            }

            if (Preview(TokenType.Dot))
            {
                MustBe(TokenType.Identifier);
                var name = _lexer.CurrentToken.Lexeme;
                if (!Preview(TokenType.RoundOpenBracket))
                {
                    MustBe(TokenType.Semicolon);
                    instruction = new PropertyCall(token.Lexeme, name);
                    return true;
                }
                var arguments = ParseArguments();
                MustBe(TokenType.RoundCloseBracket);
                MustBe(TokenType.Semicolon);
                instruction = new MethodCall(token.Lexeme, new FunctionCall(name, arguments));
                return true;
            }

            return false;
        }

        private bool TryToParseReturnInstruction(out Instruction instruction)
        {
            instruction = null;
            if (!Preview(TokenType.Return))
                return false;

            IExpression expression = null;
            if (!Preview(TokenType.Semicolon) && TryToParseExpression(out expression))
                MustBe(TokenType.Semicolon);

            instruction = new ReturnInstruction(expression);
            return true;
        }

        private IEnumerable<IExpression> ParseArguments()
        {
            var arguments = new List<IExpression>();
            while (TryToParseExpression(out var expression))
            {
                arguments.Add(expression);
                if (!Preview(TokenType.Comma))
                    break;
            }
            return arguments;
        }

        private bool TryToParseExpression(out IExpression expression)
        {
            expression = null;
            if (!TryToParseAndExpression(out var left))
                return false;

            while (Preview(TokenType.Or))
            {
                if (!TryToParseAndExpression(out var right))
                    throw new Exception("");
                left = new OrExpression(left, right);
            }

            return true;
        }

        private bool TryToParseAndExpression(out IExpression expression)
        {
            expression = null;
            if (!TryToParseRelativeExpression(out var left))
                return false;

            while (Preview(TokenType.And))
            {
                if (!TryToParseRelativeExpression(out var right))
                    throw new Exception("");
                left = new AndExpression(left, right);
            }

            return true;
        }

        private bool TryToParseRelativeExpression(out IExpression expression)
        {
            expression = null;
            if (!TryToParseAdditiveExpression(out var left))
                return false;

            while (Preview(new [] {TokenType.Less, TokenType.LessOrEqual, TokenType.Grater, TokenType.GraterOrEqual, TokenType.Equal, TokenType.NotEqual}))
            {
                var type = _lexer.CurrentToken.TokenType;
                if (!TryToParseAdditiveExpression(out var right))
                    throw new Exception("");
                left = new RelativeExpression(TokenTypeToRelativeExpressionType.Map[type], left, right);
            }

            return true;
        }

        private bool TryToParseAdditiveExpression(out IExpression expression)
        {
            expression = null;
            if (!TryToParseMultiplicativeExpression(out var left))
                return false;

            while (Preview(new [] {TokenType.Plus, TokenType.Minus}))
            {
                var type = _lexer.CurrentToken.TokenType;
                if (!TryToParseMultiplicativeExpression(out var right))
                    throw new Exception("");
                left = new AdditiveExpression(TokenTypeToAdditiveExpressionType.Map[type], left, right);
            }

            return true;
        }

        private bool TryToParseMultiplicativeExpression(out IExpression expression)
        {
            expression = null;
            if (!TryToParseNegationExpression(out var left))
                return false;

            while (Preview(new [] {TokenType.Multiplication, TokenType.Division, TokenType.Modulo}))
            {
                var type = _lexer.CurrentToken.TokenType;
                if (!TryToParseNegationExpression(out var right))
                    throw new Exception("");
                left = new MultiplicativeExpression(TokenTypeToMultiplicativeExpressionType.Map[type], left, right);
            }

            return true;
        }

        private bool TryToParseNegationExpression(out IExpression expression)
        {
            expression = null;
            var isNegated = Preview(TokenType.Not);

            if (!TryToParseLiteral(out var factor) && !TryToParseBracketExpression(out factor) && !TryToParseIdentifierExpression(out factor))
                return false;
            expression = new NotExpression(isNegated, factor);
            return true;
        }

        private bool TryToParseLiteral(out IExpression expression)
        {
            expression = null;
            return TryToParseIntLiteral(out expression) || TryToParseBoolLiteral(out expression) || TryToParseStringLiteral(out expression);
        }

        private bool TryToParseIntLiteral(out IExpression expression)
        {
            expression = null;
            var isMinus = Preview(TokenType.Minus);
            var isNumber = Preview(TokenType.IntLiteral);
            if (isMinus && !isNumber)
                throw new Exception("");
            if (!isNumber)
                return false;

            expression = new IntLiteral(int.Parse(_lexer.CurrentToken.Value));
            return true;
        }
        
        private bool TryToParseBoolLiteral(out IExpression expression)
        {
            expression = null;
            if (!Preview(TokenType.BoolLiteral))
                return false;
            expression = new BoolLiteral(_lexer.CurrentToken.Lexeme == "true");
            return true;
        }

        private bool TryToParseStringLiteral(out IExpression expression)
        {
            expression = null;
            if (!Preview(TokenType.StringLiteral))
                return false;
            expression = new StringLiteral(_lexer.CurrentToken.Lexeme);
            return true;
        }

        private bool TryToParseBracketExpression(out IExpression expression)
        {
            expression = null;
            if (!Preview(TokenType.RoundOpenBracket))
                return false;

            if (!TryToParseExpression(out expression))
                throw new Exception("");
            MustBe(TokenType.RoundCloseBracket);
            return true;
        }

        private bool TryToParseIdentifierExpression(out IExpression expression)
        {
            expression = null;
            if (!Preview(TokenType.Identifier))
                return false;

            var token = _lexer.CurrentToken;
            if (Preview(TokenType.RoundOpenBracket))
            {
                var arguments = ParseArguments();
                MustBe(TokenType.RoundCloseBracket);
                expression = new FunctionCall(token.Lexeme, arguments);
                return true;
            }

            if (Preview(TokenType.Dot))
            {
                MustBe(TokenType.Identifier);
                var name = _lexer.CurrentToken.Lexeme;
                if (!Preview(TokenType.RoundOpenBracket))
                {
                    expression = new PropertyCall(token.Lexeme, name);
                    return true;
                }
                var arguments = ParseArguments();
                MustBe(TokenType.RoundCloseBracket);
                expression = new MethodCall(token.Lexeme, new FunctionCall(name, arguments));
                return true;
            }

            expression = new Variable(token.Lexeme);
            return true;
        }

        private bool TryToParseClassDefinition(out INode classDefinition)
        {
            classDefinition = null;
            if (!Preview(TokenType.Class))
                return false;

            MustBe(TokenType.Identifier);
            var className = _lexer.CurrentToken.Lexeme;
            
            MustBe(TokenType.CurlyOpenBracket);
            
            MustBe(TokenType.Def);
            MustBe(TokenType.Init);
            MustBe(TokenType.RoundOpenBracket);
            var constructorParameters = ParseParameters();
            MustBe(TokenType.RoundCloseBracket);
            MustBe(TokenType.CurlyOpenBracket);
            var constructorInstructions = ParseInstructions();
            MustBe(TokenType.CurlyCloseBracket);

            INode definition;
            var functions = new Dictionary<string, FunctionDefinition>();
            var properties = new Dictionary<string, VarDeclaration>();
            while (TryToParseFunctionDefinition(out definition) || TryToParsePropertyDefinition(out definition))
            {
                HandleInClassDefinition(definition);
            }
            
            MustBe(TokenType.CurlyCloseBracket);

            var constructor = new FunctionDefinition(className, className, constructorParameters, constructorInstructions);
            classDefinition = new ClassDefinition(className, constructor, functions, properties);
            return true;
            
            void HandleInClassDefinition(INode definitionToHandle)
            {
                switch (definitionToHandle)
                {
                    case FunctionDefinition functionDefinition
                        when !functions.TryAdd(functionDefinition.Name, functionDefinition):
                        throw new Exception();
                    case VarDeclaration propertyDefinition
                        when !properties.TryAdd(propertyDefinition.Name, propertyDefinition):
                        throw new Exception();
                }
            }
        }

        private bool TryToParsePropertyDefinition(out INode propertyDefinition)
        {
            propertyDefinition = null;
            if (!Preview(new[] {TokenType.Int, TokenType.Bool, TokenType.Identifier}))
                return false;

            var type = TokenToType.Map(_lexer.CurrentToken);
            MustBe(TokenType.Identifier);
            var name = _lexer.CurrentToken.Lexeme;

            if (!Preview(TokenType.Assign))
            {
                MustBe(TokenType.Semicolon);
                propertyDefinition = new VarDeclaration(name, type, null);
                return true;
            }

            IExpression literal;
            var isInt = TryToParseIntLiteral(out literal);
            var isBool = false;
            if (!isInt)
                isBool = TryToParseIntLiteral(out literal);
            if (!isInt && !isBool)
                throw new Exception("");
            var assignType =  TokenToType.Map(_lexer.CurrentToken);
            if (type != assignType)
                throw new Exception("");
            
            MustBe(TokenType.Semicolon);
            propertyDefinition = new VarDeclaration(name, type, literal);
            return true;
        }

        private bool Preview(TokenType tokenType)
        {
            var token = _lexer.GetNextToken();
            if (token.TokenType == tokenType) 
                return true;
            _lexer.RollbackToken(token);
            return false;
        }

        private bool Preview(IEnumerable<TokenType> tokenTypes)
        {
            var token = _lexer.GetNextToken();
            if (tokenTypes.Contains(token.TokenType))
                return true;
            _lexer.RollbackToken(token);
            return false;
        }

        private void MustBe(TokenType tokenType)
        {
            var token = _lexer.GetNextToken();
            if (token.TokenType == tokenType) 
                return;
            throw new Exception($"ERROR: Expected token type: {tokenType}, Current token type: {token.TokenType} [line: {token.Line} column: {token.Position}]");
        }

        private void MustBe(IEnumerable<TokenType> tokenTypes)
        {
            var token = _lexer.GetNextToken();
            if (tokenTypes.Contains(token.TokenType)) return;
            throw new Exception($"ERROR: Wrong token type [line: {token.Line} column: {token.Position}]");
        }
    }
}