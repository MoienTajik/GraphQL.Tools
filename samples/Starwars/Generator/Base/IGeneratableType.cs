using System.Collections.Generic;

namespace Starwars.Generator.Base
{
    public interface IGeneratableType
    {
        string Name { get; }

        List<Property> Properties { get; }
    }
}