using GraphQL.Types;
using Starwars.Generator.Base;
using System.Collections.Generic;

namespace Starwars.Generator.Extractors
{
    public interface IGeneratableTypeExtractor
    {
        /// <summary>
        /// This method will find and extract specified generatable types from GraphQL types.
        /// </summary>
        /// <param name="graphTypes">GraphQL types to lookup.</param>
        /// <returns>Extracted generatable types.</returns>
        IEnumerable<IGeneratableType> Extract(IEnumerable<IGraphType> graphTypes);
    }
}