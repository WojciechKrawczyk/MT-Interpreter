namespace Interpreter.ParserModule.Structures.Expressions.Literals
{
    public class StringLiteral : IExpression
    {
        public string Value { get; }

        public StringLiteral(string value)
        {
            Value = value;
        }
    }
}