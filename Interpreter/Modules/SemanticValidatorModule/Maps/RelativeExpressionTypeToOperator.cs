using System.Collections.Generic;
using Interpreter.Modules.ParserModule.Structures.Expressions.Types;

namespace Interpreter.Modules.SemanticValidatorModule.Maps
{
    public static class RelativeExpressionTypeToOperator
    {
        public static readonly Dictionary<RelativeExpressionType, string> Map = new()
        {
            {RelativeExpressionType.Less, "<"},
            {RelativeExpressionType.LessOrEqual, "<="},
            {RelativeExpressionType.Grater, ">"},
            {RelativeExpressionType.GraterOrEqual, ">="},
            {RelativeExpressionType.NotEqual, "!="},
            {RelativeExpressionType.Equal, "=="}
        };
    }
}