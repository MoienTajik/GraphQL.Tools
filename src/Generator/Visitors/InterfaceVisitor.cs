using System.Collections.Generic;
using System.Linq;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Tools.Generator.Visitors
{
    /// <summary>
    /// This visitor will extract GraphQL interfaces.
    /// </summary>
    /// <example>
    /// GraphQL schema:
    /// <code>
    /// interface ICharacter {
    ///   id: Int!
    ///   name: String!
    /// }
    /// 
    /// type Human implements ICharacter {
    ///   id: Int!
    ///   name: String!
    ///   totalCredits: Int
    /// }
    /// </code>
    ///
    /// Extracted interfaces and class:
    /// <code>
    /// public interface ICharacter
    /// {
    /// 	public int Id { get; set; }
    /// 	public string Name { get; set; }
    /// }
    /// 
    /// public class Human : ICharacter
    /// {
    /// 	public int Id { get; set; }
    /// 	public string Name { get; set; }
    /// 	public int? TotalCredits { get; set; }
    /// }
    /// </code>
    /// </example>
    public class InterfaceVisitor : IGeneratableTypeVisitor
    {
        public HashSet<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
        {
            var @interfaces = new HashSet<IGeneratableType>();

            foreach (InterfaceGraphType interfaceGraphType in graphTypes.Where(type => type is InterfaceGraphType))
            {
                var interfaceName = interfaceGraphType.Name;
                var @interface = new Interface(interfaceName);

                foreach (FieldType fieldType in interfaceGraphType.Fields)
                {
                    var propertyName = fieldType.Name;

                    var propertyType = "";
                    if (fieldType.ResolvedType.GetNamedType() is GraphQLTypeReference typeReference)
                    {
                        propertyType = typeReference.TypeName;
                    }
                    else if (fieldType.ResolvedType.GetNamedType() is GraphType type)
                    {
                        propertyType = type.Name;
                    }

                    var isNullable = (fieldType.Metadata.First().Value as GraphQLFieldDefinition)!.Type!.Kind != ASTNodeKind.NonNullType;
                    @interface.Properties.Add(new Property(propertyName, propertyType, isNullable));
                }

                @interfaces.Add(@interface);
            }

            return @interfaces;
        }
    }
}