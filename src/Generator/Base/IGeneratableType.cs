using System.Collections.Generic;

namespace GraphQL.Tools.Generator.Base
{
    public interface IGeneratableType
    {
        string Name { get; }

        List<IMember> Properties { get; }
    }
}