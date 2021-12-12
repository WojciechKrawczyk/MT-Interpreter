using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Lexers;
using Interpreter.ParserModule.Nodes;
using Interpreter.ParserModule.Nodes.Instructions;
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
            MustBe(TokenType.CurlyOpenBracket, "");
            MustBe(TokenType.Program, "");
            INode definition;
            while (TryToParseFunctionDefinition(out definition) || TryToParseClassDefinition(out definition))
            {
                HandleDefinition(definition);
            }
            MustBe(TokenType.CurlyCloseBracket, "");
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
            var token = _lexer.GetNextToken();
            if (token.TokenType != TokenType.Def)
            {
                _lexer.RollbackToken(token);
                return false;
            }

            MustBe(new [] {TokenType.Int, TokenType.Bool, TokenType.Void, TokenType.Identifier}, "");
            var type = _lexer.CurrentToken;

            MustBe(TokenType.Identifier, "");
            var name = _lexer.CurrentToken;
            
            MustBe(TokenType.RoundOpenBracket, "");
            var parameters = TryToParseParameters();
            MustBe(TokenType.RoundCloseBracket, "");
            
            MustBe(TokenType.CurlyOpenBracket, "");
            //var instructions = 
            MustBe(TokenType.CurlyCloseBracket, "");

            functionDefinition = new FunctionDefinition();
            return true;
        }

        private IEnumerable<Parameter> TryToParseParameters()
        {
            var parameters = new List<Parameter>();
            var token = _lexer.GetNextToken();
            var availableTokenTypes = new[] {TokenType.Int, TokenType.Bool, TokenType.Identifier};
            while (availableTokenTypes.Contains(token.TokenType))
            {
                MustBe(TokenType.Identifier, "");
                parameters.Add(new Parameter(TokenTypeToType.Map[token.TokenType], _lexer.CurrentToken.Lexeme));
                token = _lexer.GetNextToken();
            }
            _lexer.RollbackToken(token);
            return parameters;
        }

        private IEnumerable<Instruction> TryToParseFunctionBlock()
        {
            MustBe(TokenType.CurlyOpenBracket, "");
            var instructions = TryToParseInstructions();
            MustBe(TokenType.Return, "");


            instructions = instructions.Append(new ReturnInstruction());
            MustBe(TokenType.CurlyCloseBracket, "");
            return instructions;
        }

        private IEnumerable<Instruction> TryToParseInstructions()
        {
            var instructions = new List<Instruction>();
            Instruction instruction;
            while (TryToParseIfInstruction(out instruction)
                   || TryToParseWhileInstruction(out instruction)
                   || TryToParseFlatInitInstructions(out instruction)
                   || TryToParseIdentifierInstruction(out instruction))
            {
                instructions.Add(instruction);
            }
            return instructions;
        }

        private bool TryToParseIfInstruction(out Instruction instruction)
        {
            instruction = null;
            var token = _lexer.GetNextToken();
            if (token.TokenType != TokenType.If)
                return false;
            
            MustBe(TokenType.RoundOpenBracket, "");
            //var expression = 
            MustBe(TokenType.RoundCloseBracket, "");
            
            MustBe(TokenType.CurlyOpenBracket, "");
            var instructions = TryToParseInstructions();
            MustBe(TokenType.CurlyCloseBracket, "");
            
            //var elseIn = 
            
            instruction = new IfInstruction();
            return true;
        }

        private bool TryToParseWhileInstruction(out Instruction instruction)
        {
            instruction = new WhileInstruction();
            return true;
        }

        private bool TryToParseFlatInitInstructions(out Instruction instruction)
        {
            instruction = new InitInstruction();
            return true;
        }

        private bool TryToParseIdentifierInstruction(out Instruction instruction)
        {
            instruction = null;
            return false;
        }







        private bool TryToParseClassDefinition(out INode classDefinition)
        {
            classDefinition = new ClassDefinition();
            return false;
        }
        
        private void MustBe(TokenType tokenType, string message)
        {
            var token = _lexer.GetNextToken();
            if (token.TokenType != tokenType)
                throw new Exception(message);
        }

        private void MustBe(IEnumerable<TokenType> tokenTypes, string message)
        {
            var token = _lexer.GetNextToken();
            if (!tokenTypes.Contains(token.TokenType))
                throw new Exception(message);
        }
    }
}