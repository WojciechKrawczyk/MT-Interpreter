namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class IntLiteral : IExpression
    {
        public int Value { get; }

        public IntLiteral(int value)
        {
            Value = value;
        }
    }
}