﻿namespace Interpreter.SemanticValidator
{
    public class DefinedVariable
    {
        public string Type { get; }
        public string Name { get; }
        public bool IsInitialized { get; private set; }

        public DefinedVariable(string type, string name, bool isInitialized)
        {
            Type = type;
            Name = name;
            IsInitialized = isInitialized;
        }

        public void InitializeVariable() => IsInitialized = true;
    }
}