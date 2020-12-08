using System.Collections.Generic;
using System.Linq;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Tools.Generator.Extractors
{
    /// <summary>
    /// This extractor will extract GraphQL fields as classes.
    /// </summary>
    /// <example>
    /// GraphQL schema:
    /// <code>
    ///  type Simple {
    ///      int32: Int!
    ///      float: Float
    ///      string: String!
    ///      bool: Boolean
    ///      id: ID!
    ///  }
    /// </code>
    ///
    /// Extracted class:
    /// <code>
    /// public class Simple
    /// {
    ///     public int Int32 { get; set; }
    ///     public float? Float { get; set; }
    ///     public string String { get; set; }
    ///     public bool? Bool { get; set; }
    ///     public Guid Id { get; set; }
    /// }
    /// </code>
    /// </example>
    public class ClassExtractor : IGeneratableTypeExtractor
    {
        public IEnumerable<IGeneratableType> Extract(IEnumerable<IGraphType> graphTypes)
        {
            foreach (ObjectGraphType objectGraphType in graphTypes.Where(type => type is ObjectGraphType))
            {
                var className = objectGraphType.Name;
                var @class = new Class(className);

                foreach (FieldType fieldType in objectGraphType.Fields)
                {
                    var propertyName = fieldType.Name;

                    var propertyType = "";
                    if (fieldType.ResolvedType.GetNamedType() is GraphQLTypeReference reference)
                    {
                        propertyType = reference.TypeName;
                    }
                    else if (fieldType.ResolvedType.GetNamedType() is GraphType type)
                    {
                        propertyType = type.Name;
                    }

                    var isNullable = (fieldType.Metadata.First().Value as GraphQLFieldDefinition)!.Type!.Kind != ASTNodeKind.NonNullType;
                    @class.Properties.Add(new Property(propertyName, propertyType, isNullable));
                }

                ExtractImplementedInterfaces(objectGraphType, @class);

                yield return @class;
            }
        }

        private static void ExtractImplementedInterfaces(ObjectGraphType objectGraphType, Class @class)
        {
            if (objectGraphType.ResolvedInterfaces.Any())
            {
                foreach (var interfaceGraphType in objectGraphType.ResolvedInterfaces)
                {
                    var interfaceName = ((GraphQLTypeReference)interfaceGraphType).TypeName;
                    @class.Interfaces.Add(interfaceName);
                }
            }
        }
    }
}