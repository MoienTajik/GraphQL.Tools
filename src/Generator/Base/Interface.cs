using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Tools.Generator.Base
{
    public class Interface : IGeneratableType
    {
        public Interface(string name)
        {
            Name = name;
            Properties = new List<IMember>();
        }

        public string Name { get; }

        public List<IMember> Properties { get; }

        public override string ToString()
        {
            var properties = string.Join(Environment.NewLine, Properties.Select(prop => prop.ToString()));

            return $@"
                public interface {Name}
                {{
                    {properties}
                }}
                ";
        }
    }
}