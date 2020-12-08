using System;
using System.Collections.Generic;
using System.Linq;

namespace Starwars.Generator.Base
{
    public class Class : IGeneratableType
    {
        public Class(string name)
        {
            Name = name;
            Properties = new List<Property>();
            Interfaces = new List<string>();
        }

        public string Name { get; }

        public List<Property> Properties { get; }

        public List<string> Interfaces { get; }

        public bool HasInterface => Interfaces.Any();

        public override string ToString()
        {
            var properties = string.Join(Environment.NewLine, Properties.Select(prop => prop.ToString()));

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