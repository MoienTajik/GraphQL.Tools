using System.Collections.Generic;
using System.Linq;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;

namespace GraphQL.Tools.Generator.Visitors
{
    /// <summary>
    /// This visitor will extract GraphQL enums.
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
    public class EnumVisitor : IGeneratableTypeVisitor
    {
        public HashSet<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
        {
            var @enums = new HashSet<IGeneratableType>();

            foreach (EnumerationGraphType enumGraphType in graphTypes.Where(type => type is EnumerationGraphType))
            {
                var @enum = new Enum(enumGraphType.Name);

                foreach (EnumValueDefinition enumValueDefinition in enumGraphType.Values)
                {
                    var value = enumValueDefinition.Name;

                    @enum.Properties.Add(new EnumValue(value));
                }

                @enums.Add(@enum);
            }

            return @enums;
        }
    }
}