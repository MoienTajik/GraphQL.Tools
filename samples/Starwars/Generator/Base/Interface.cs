using System;
using System.Collections.Generic;
using System.Linq;

namespace Starwars.Generator.Base
{
    public class Interface : IGeneratableType
    {
        public Interface(string name)
        {
            Name = name;
            Properties = new List<Property>();
        }

        public string Name { get; }

        public List<Property> Properties { get; }

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