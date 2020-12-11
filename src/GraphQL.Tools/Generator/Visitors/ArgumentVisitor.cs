using GraphQL.Tools.Generator.Base;
using GraphQL.Tools.Generator.Extensions;
using GraphQL.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraphQL.Tools.Generator.Visitors
{
    /// <summary>
    /// This visitor will extract GraphQL field arguments as a class.
    /// </summary>
    /// <example>
    /// GraphQL schema:
    /// <code>
    /// type Query {
    ///     order(id: Int!): Order!
    /// }
    /// </code>
    ///
    /// Extracted class:
    /// <code>
    /// public class Query_Order_Arguments
    /// {
    ///     public int Id { get; set; }
    /// }
    /// </code>
    /// </example>
    public class ArgumentVisitor : IGeneratableTypeVisitor
    {
        public HashSet<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
        {
            var @classes = new HashSet<IGeneratableType>();

            foreach (ObjectGraphType objectGraphType in graphTypes.Where(type => type is ObjectGraphType))
            {
                var className = objectGraphType.Name; // ex: Query

                foreach (FieldType fieldType in objectGraphType.Fields)
                {
                    var fieldName = fieldType.Name.ToPascalCase(); // ex: Order

                    if (fieldType.Arguments.Any())
                    {
                        var @class = new Class($"{className}_{fieldName}_Arguments"); // ex: Query_Order_Arguments

                        foreach (QueryArgument? argument in fieldType.Arguments)
                        {
                            @class.Properties
                                .Add(new Property(argument.Name,
                                    argument.GetTypeName(),
                                    argument.IsArray(),
                                    argument.IsNullable()));

                            @classes.Add(@class);
                        }
                    }
                }
            }

            return @classes;
        }
    }
}