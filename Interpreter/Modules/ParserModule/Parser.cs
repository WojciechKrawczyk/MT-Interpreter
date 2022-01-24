using System.Collections.Generic;
using System.Linq;
using Interpreter.Modules.ErrorsHandlerModule;
using Interpreter.Modules.LexerModule;
using Interpreter.Modules.LexerModule.Tokens;
using Interpreter.Modules.ParserModule.Structures;
using Interpreter.Modules.ParserModule.Structures.Definitions;
using Interpreter.Modules.ParserModule.Structures.Expressions;
using Interpreter.Modules.ParserModule.Structures.Expressions.Literals;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types.Maps;
using Interpreter.Modules.ParserModule.Structures.Instructions;
using Interpreter.Modules.ParserModule.Types;

namespace Interpreter.Modules.ParserModule
{
    public class Parser
    {
        private readonly ILexer _lexer;
        private readonly ErrorsHandler _errorsHandler;
        private readonly List<FunctionDefinition> _functions = new();
        private readonly List<ClassDefinition> _classes = new();
        private bool _errorOccured = false;

        public Parser(ILexer lexer, ErrorsHandler errorsHandler)
        {
            _lexer = lexer;
            _errorsHandler = errorsHandler;
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
            if (!_errorOccured)
            {
                return true;
            }
            _errorsHandler.StopInterpretation();
            return false;
        }

        private void HandleDefinition(INode definition)
        {
            switch (definition)
            {
                case FunctionDefinition functionDefinition:
                    _functions.Add(functionDefinition);
                    break;
                case ClassDefinition classDefinition:
                    _classes.Add(classDefinition);
                    break;
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

        private IEnumerable<FunctionDefinition.Parameter> ParseParameters()
        {
            var parameters = new List<FunctionDefinition.Parameter>();
            var availableTypes = new[] {TokenType.Int, TokenType.Bool, TokenType.Identifier};
            if (!Preview(availableTypes))
                return parameters;

            var type = TokenToType.Map(_lexer.CurrentToken);
            MustBe(TokenType.Identifier);
            var name = _lexer.CurrentToken.Lexeme;
            parameters.Add(new FunctionDefinition.Parameter(type, name));
            while (Preview(TokenType.Comma))
            {
                MustBe(availableTypes);
                type = TokenToType.Map(_lexer.CurrentToken);
                MustBe(TokenType.Identifier);
                name = _lexer.CurrentToken.Lexeme;
                parameters.Add(new FunctionDefinition.Parameter(type, name));
            }
            return parameters;
        }

        private IEnumerable<IInstruction> ParseInstructions()
        {
            var instructions = new List<IInstruction>();
            IInstruction instruction;
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

        private bool TryToParseIfInstruction(out IInstruction instruction)
        {
            instruction = null;
            if (!Preview(TokenType.If))
                return false;

            MustBe(TokenType.RoundOpenBracket);
            if (!TryToParseExpression(out var condition))
                _errorsHandler.HandleFatalError($"Unable to parse 'condition' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
            MustBe(TokenType.RoundCloseBracket);

            MustBe(TokenType.CurlyOpenBracket);
            var instructions = ParseInstructions();
            MustBe(TokenType.CurlyCloseBracket);

            IEnumerable<IInstruction> elseInstructions = null;
            if (Preview(TokenType.Else))
            {
                MustBe(TokenType.CurlyOpenBracket);
                elseInstructions = ParseInstructions();
                MustBe(TokenType.CurlyCloseBracket);
            }

            instruction = new IfInstruction(condition, instructions, elseInstructions);
            return true;
        }

        private bool TryToParseWhileInstruction(out IInstruction instruction)
        {
            instruction = null;
            if (!Preview(TokenType.While))
                return false;

            MustBe(TokenType.RoundOpenBracket);
            if (!TryToParseExpression(out var condition))
                _errorsHandler.HandleFatalError($"Unable to parse 'condition' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
            MustBe(TokenType.RoundCloseBracket);
            
            MustBe(TokenType.CurlyOpenBracket);
            var instructions = ParseInstructions();
            MustBe(TokenType.CurlyCloseBracket);

            instruction = new WhileInstruction(condition, instructions);
            return true;
        }

        private bool TryToParseFlatInitInstruction(out IInstruction instruction)
        {
            instruction = null;
            if (!Preview(new[] {TokenType.Int, TokenType.Bool}))
                return false;

            var type = TokenToType.Map(_lexer.CurrentToken);
            MustBe(TokenType.Identifier);
            var name = _lexer.CurrentToken.Lexeme;
            IExpression expression = null;
            if (Preview(TokenType.Assign) && !TryToParseExpression(out expression))
                _errorsHandler.HandleFatalError($"Unable to parse 'assign' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
            MustBe(TokenType.Semicolon);
            
            instruction = new VarDeclaration(name, type, expression);
            return true;
        }

        private bool TryToParseIdentifierInstruction(out IInstruction instruction)
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
                    _errorsHandler.HandleFatalError($"Unable to parse 'assign' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                MustBe(TokenType.Semicolon);
                instruction = new VarDeclaration(name, TokenToType.Map(token), expression);
                return true;
            }

            if (Preview(TokenType.Assign))
            {
                if (!TryToParseExpression(out var expression))
                    _errorsHandler.HandleFatalError($"Unable to parse 'assign' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                MustBe(TokenType.Semicolon);
                instruction = new Assignment(token.Lexeme, expression);
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
                MustBe(TokenType.RoundOpenBracket);
                var arguments = ParseArguments();
                MustBe(TokenType.RoundCloseBracket);
                MustBe(TokenType.Semicolon);
                instruction = new MethodCall(token.Lexeme, new FunctionCall(name, arguments));
                return true;
            }

            return false;
        }

        private bool TryToParseReturnInstruction(out IInstruction instruction)
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
            if (!TryToParseAndExpression(out expression))
                return false;

            while (Preview(TokenType.Or))
            {
                if (!TryToParseAndExpression(out var right))
                    _errorsHandler.HandleFatalError($"Unable to parse 'and' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                expression = new OrExpression(expression, right);
            }

            return true;
        }

        private bool TryToParseAndExpression(out IExpression expression)
        {
            if (!TryToParseRelativeExpression(out expression))
                return false;

            while (Preview(TokenType.And))
            {
                if (!TryToParseRelativeExpression(out var right))
                    _errorsHandler.HandleFatalError($"Unable to parse 'relative' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                expression = new AndExpression(expression, right);
            }

            return true;
        }

        private bool TryToParseRelativeExpression(out IExpression expression)
        {
            if (!TryToParseAdditiveExpression(out expression))
                return false;

            while (Preview(new [] {TokenType.Less, TokenType.LessOrEqual, TokenType.Grater, TokenType.GraterOrEqual, TokenType.Equal, TokenType.NotEqual}))
            {
                var type = _lexer.CurrentToken.TokenType;
                if (!TryToParseAdditiveExpression(out var right))
                    _errorsHandler.HandleFatalError($"Unable to parse 'additive' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                expression = new RelativeExpression(TokenTypeToRelativeExpressionType.Map[type], expression, right);
            }

            return true;
        }

        private bool TryToParseAdditiveExpression(out IExpression expression)
        {
            if (!TryToParseMultiplicativeExpression(out expression))
                return false;

            while (Preview(new [] {TokenType.Plus, TokenType.Minus}))
            {
                var type = _lexer.CurrentToken.TokenType;
                if (!TryToParseMultiplicativeExpression(out var right))
                    _errorsHandler.HandleFatalError($"Unable to parse 'multiplicative' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                expression = new AdditiveExpression(TokenTypeToAdditiveExpressionType.Map[type], expression, right);
            }

            return true;
        }

        private bool TryToParseMultiplicativeExpression(out IExpression expression)
        {
            if (!TryToParseNegationExpression(out expression))
                return false;

            while (Preview(new [] {TokenType.Multiplication, TokenType.Division, TokenType.Modulo}))
            {
                var type = _lexer.CurrentToken.TokenType;
                if (!TryToParseNegationExpression(out var right))
                    _errorsHandler.HandleFatalError($"Unable to parse 'not' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
                expression = new MultiplicativeExpression(TokenTypeToMultiplicativeExpressionType.Map[type], expression, right);
            }

            return true;
        }

        private bool TryToParseNegationExpression(out IExpression expression)
        {
            var isNegated = Preview(TokenType.Not);
            if (!TryToParseLiteral(out expression) && !TryToParseBracketExpression(out expression) && !TryToParseIdentifierExpression(out expression))
                return false;
            if (isNegated) 
                expression = new NotExpression(expression);

            return true;
        }

        private bool TryToParseLiteral(out IExpression expression)
        {
            return TryToParseIntLiteral(out expression) || TryToParseBoolLiteral(out expression) || TryToParseStringLiteral(out expression);
        }

        private bool TryToParseIntLiteral(out IExpression expression)
        {
            expression = null;
            var isMinus = Preview(TokenType.Minus);
            var isNumber = Preview(TokenType.IntLiteral);
            if (isMinus && !isNumber)
                _errorsHandler.HandleFatalError($"Unable to parse 'int literal' expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
            if (!isNumber)
                return false;

            expression = new IntLiteral(int.Parse(_lexer.CurrentToken.Value));
            return true;
        }
        
        private bool TryToParseBoolLiteral(out IExpression expression)
        {
            expression = null;
            if (Preview(TokenType.False))
            {
                expression = new BoolLiteral(false);
                return true;
            }
            if (Preview(TokenType.True))
            {
                expression = new BoolLiteral(true);
                return true;
            }
            return false;
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
                _errorsHandler.HandleFatalError($"Unable to parse expression [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");
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
                expression = new FunctionCallExpression(token.Lexeme, arguments);
                return true;
            }

            if (Preview(TokenType.Dot))
            {
                MustBe(TokenType.Identifier);
                var name = _lexer.CurrentToken.Lexeme;
                if (!Preview(TokenType.RoundOpenBracket))
                {
                    expression = new PropertyCallExpression(token.Lexeme, name);
                    return true;
                }
                var arguments = ParseArguments();
                MustBe(TokenType.RoundCloseBracket);
                expression = new MethodCallExpression(token.Lexeme, new FunctionCall(name, arguments));
                return true;
            }

            expression = new VariableExpression(token.Lexeme);
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
            var functions = new List<FunctionDefinition>();
            var properties = new List<VarDeclaration>();
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
                    case FunctionDefinition functionDefinition:
                        functions.Add(functionDefinition);
                        break;
                    case VarDeclaration propertyDefinition:
                        properties.Add(propertyDefinition);
                        break;
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
                isBool = TryToParseBoolLiteral(out literal);
            if (!isInt && !isBool)
                _errorsHandler.HandleFatalError($"Unable to initialize property with type '{type}' in declaration [Line: {_lexer.CurrentToken.Line}, Position: {_lexer.CurrentToken.Position}]");

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
            if (token.TokenType != tokenType)
                _errorsHandler.HandleFatalError($"Invalid token type, expected '{tokenType}', current type '{token.TokenType}' [Line: {token.Line}, Position: {token.Position}]");
        }

        private void MustBe(IEnumerable<TokenType> tokenTypes)
        {
            var token = _lexer.GetNextToken();
            if (!tokenTypes.Contains(token.TokenType))
                _errorsHandler.HandleFatalError($"Invalid token type [Line: {token.Line}, Position: {token.Position}]");
        }
    }
}