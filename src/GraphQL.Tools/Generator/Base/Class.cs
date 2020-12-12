using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Tools.Generator.Base
{
    public class Class : IGeneratableType
    {
        public Class(string name)
        {
            Name = name;
            Properties = new HashSet<IMember>();
            Interfaces = new HashSet<string>();
        }

        public string Name { get; }

        public HashSet<IMember> Properties { get; }

        public HashSet<string> Interfaces { get; }

        public bool HasInterface => Interfaces.Any();

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            var separator = $"{Environment.NewLine}            ";
            var properties = string.Join(separator, Properties.Select(prop => prop.ToString()));

            if (HasInterface)
            {
                var interfaces = string.Join(", ", Interfaces);
                return $@"
        public class {Name} : {interfaces}
        {{
            {properties}
        }}
        ";
            }
            else
            {
                return $@"
        public class {Name}
        {{
            {properties}
        }}
        ";
            }
        }
    }
}