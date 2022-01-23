using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;

namespace Interpreter.Modules.SemanticValidatorModule.Maps
{
    public static class MultiplicativeExpressionTypeToOperator
    {
        public static readonly Dictionary<MultiplicativeExpressionType, string> Map = new()
        {
            {MultiplicativeExpressionType.Multiplication, "*"},
            {MultiplicativeExpressionType.Division, "/"},
            {MultiplicativeExpressionType.Modulo, "%"}
        };
    }
}