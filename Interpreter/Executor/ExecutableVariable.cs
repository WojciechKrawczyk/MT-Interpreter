using System.Collections.Generic;

namespace Interpreter.Executor
{
    public class ExecutableVariable
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Dictionary<string, ExecutableVariable> Properties { get; set; }

        public ExecutableVariable Clone()
        {
            var newExecutableVariable = new ExecutableVariable
            {
                Type = Type,
                Name = Name,
                Value = Value
            };
            foreach (var property in Properties.Values)
            {
                newExecutableVariable.Properties.Add(property.Name, property.Clone());
            }

            return newExecutableVariable;
        }
    }
}