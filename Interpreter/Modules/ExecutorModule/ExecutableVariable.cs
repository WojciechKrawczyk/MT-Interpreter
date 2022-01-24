using System.Collections.Generic;

namespace Interpreter.Modules.ExecutorModule
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
                Value = Value,
                Properties = new Dictionary<string, ExecutableVariable>()
            };
            if (Properties != null)
            {
                foreach (var (key, value) in Properties)
                {
                    newExecutableVariable.Properties.Add(key, value.Clone());
                }
            }

            return newExecutableVariable;
        }
    }
}