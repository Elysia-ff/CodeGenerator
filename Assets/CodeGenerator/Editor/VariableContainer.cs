using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator
{
    [Serializable]
    public class VariableContainer
    {
        // Ensure this class can be serialized for editor hot reload
        // https://docs.unity3d.com/Manual/script-Serialization.html

        private Variable[] variables;

        public int Count { get { return variables != null ? variables.Length : 0; } }

        public Variable this[int i]
        {
            get
            {
                if (variables == null)
                    throw new NullReferenceException();
                if (i < 0 || i >= variables.Length)
                    throw new IndexOutOfRangeException();

                return variables[i];
            }
            set
            {
                if(variables == null)
                    throw new NullReferenceException();
                if (i < 0 || i >= variables.Length)
                    throw new IndexOutOfRangeException();

                variables[i] = value;
            }
        }

        public VariableContainer(IEnumerable<Variable> source)
        {
            variables = source.ToArray();
        }
    }
    
    [Serializable]
    public struct Variable
    {
        public int line;
        public string key;
        public string value;

        public Variable(int _line, string _key)
        {
            line = _line;
            key = _key;
            value = string.Empty;
        }
    }
}
