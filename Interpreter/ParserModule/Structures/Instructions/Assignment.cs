using Interpreter.ParserModule.Structures.Expressions;

namespace Interpreter.ParserModule.Structures.Instructions
{
    public class Assignment : Instruction
    {
        public Variable Variable { get; }
        public IExpression Expression { get; }
        
        public Assignment(Variable variable, IExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }
    }
}