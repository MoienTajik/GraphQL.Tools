using System.Collections.Generic;
using System.Linq;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Tools.Generator.Extractors
{
    /// <summary>
    /// This extractor will extract GraphQL field arguments as a class.
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
    public class ArgumentExtractor : IGeneratableTypeExtractor
    {
        public IEnumerable<IGeneratableType> Extract(IEnumerable<IGraphType> graphTypes)
        {
            foreach (ObjectGraphType objectGraphType in graphTypes.Where(type => type is ObjectGraphType))
            {
                var className = objectGraphType.Name; // ex: Query

                foreach (FieldType fieldType in objectGraphType.Fields)
                {
                    var fieldName = fieldType.Name.ToPascalCase(); // ex: Order

                    if (fieldType.Arguments.Any())
                    {
                        var @class = new Class($"{className}_{fieldName}_Arguments"); // ex: Query_Order_Arguments

                        foreach (var argument in fieldType.Arguments)
                        {
                            var propertyName = argument.Name;

                            var propertyType = "";
                            if (argument.ResolvedType.GetNamedType() is GraphQLTypeReference typeReference)
                            {
                                propertyType = typeReference.TypeName;
                            }
                            else if (argument.ResolvedType.GetNamedType() is GraphType type)
                            {
                                propertyType = type.Name;
                            }

                            var isNullable = (argument.Metadata.First().Value as GraphQLInputValueDefinition)!.Type!.Kind != ASTNodeKind.NonNullType;
                            @class.Properties.Add(new Property(propertyName, propertyType, isNullable));

                            yield return @class;
                        }
                    }
                }
            }
        }
    }
}