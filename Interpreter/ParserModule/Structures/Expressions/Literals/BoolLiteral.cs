namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class BoolLiteral : IExpression
    {
        public bool Value { get; }

        public BoolLiteral(bool value)
        {
            Value = value;
        }
    }
}