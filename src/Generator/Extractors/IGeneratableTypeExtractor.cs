using System.Collections.Generic;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;

namespace GraphQL.Tools.Generator.Extractors
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