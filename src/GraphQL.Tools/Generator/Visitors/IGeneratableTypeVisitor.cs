using System.Collections.Generic;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;

namespace GraphQL.Tools.Generator.Visitors
{
    public interface IGeneratableTypeVisitor
    {
        /// <summary>
        /// This method will visit and extract specified generatable types from GraphQL types.
        /// </summary>
        /// <param name="graphTypes">GraphQL types to visit.</param>
        /// <returns>Extracted generatable types.</returns>
        HashSet<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes);
    }
}