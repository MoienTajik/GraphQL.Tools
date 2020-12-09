using System.Collections.Generic;

namespace GraphQL.Tools.Generator.Base
{
    public interface IGeneratableType
    {
        string Name { get; }

        HashSet<IMember> Properties { get; }
    }
}