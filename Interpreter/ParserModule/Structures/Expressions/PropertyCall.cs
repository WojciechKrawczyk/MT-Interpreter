namespace Interpreter.ParserModule.Structures.Expressions
{
    public class PropertyCall : IExpression
    {
        public string ObjectName { get; }
        public string PropertyName { get; }

        public PropertyCall(string objectName, string propertyName)
        {
            ObjectName = objectName;
            PropertyName = propertyName;
        }
    }
}