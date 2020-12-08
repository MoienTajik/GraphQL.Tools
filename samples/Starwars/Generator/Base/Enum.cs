using System;
using System.Collections.Generic;
using System.Linq;

namespace Starwars.Generator.Base
{
    public class Enum : IGeneratableType
    {
        public Enum(string name)
        {
            Name = name;
            Properties = new List<IMember>();
        }

        public string Name { get; }

        public List<IMember> Properties { get; }

        public override string ToString()
        {
            var properties = string.Join($",{Environment.NewLine}", Properties.Select(prop => prop.ToString()));

            return $@"
                public enum {Name}
                {{
                    {properties}
                }}
                ";
        }
    }
}