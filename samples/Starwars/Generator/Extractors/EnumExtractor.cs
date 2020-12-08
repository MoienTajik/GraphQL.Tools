using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using Starwars.Generator.Base;

namespace Starwars.Generator.Extractors
{
    /// <summary>
    /// This extractor will extract GraphQL enums.
    /// </summary>
    /// <example>
    /// GraphQL schema:
    /// <code>
    /// enum Color {
    ///   RED
    ///   GREEN
    ///   BLUE
    /// }
    /// </code>
    ///
    /// Extracted enum:
    /// <code>
    /// public enum Color
    /// {
    /// 	RED,
    ///     GREEN,
    ///     BLUE
    /// }
    /// </code>
    /// </example>
    public class EnumExtractor : IGeneratableTypeExtractor
    {
        public IEnumerable<IGeneratableType> Extract(IEnumerable<IGraphType> graphTypes)
        {
            foreach (EnumerationGraphType enumGraphType in graphTypes.Where(type => type is EnumerationGraphType))
            {
                var @enum = new Enum(enumGraphType.Name);

                foreach (EnumValueDefinition enumValueDefinition in enumGraphType.Values)
                {
                    var value = enumValueDefinition.Name;

                    @enum.Properties.Add(new EnumValue(value));
                }

                yield return @enum;
            }
        }
    }
}