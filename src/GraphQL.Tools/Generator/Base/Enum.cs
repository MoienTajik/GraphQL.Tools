using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Tools.Generator.Base
{
    public class Enum : IGeneratableType
    {
        public Enum(string name)
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
            var separator = $",{Environment.NewLine}            ";
            var properties = string.Join(separator, Properties.Select(prop => prop.ToString()));

            return $@"
        public enum {Name}
        {{
            {properties}
        }}
        ";
        }
    }
}