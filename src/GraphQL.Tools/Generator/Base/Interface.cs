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
            Properties = new HashSet<IMember>();
        }

        public string Name { get; }

        public HashSet<IMember> Properties { get; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            var separator = $"{Environment.NewLine}            ";
            var properties = string.Join(separator, Properties.Select(prop => prop.ToString()));

            return $@"
        public interface {Name}
        {{
            {properties}
        }}
        ";
        }
    }
}