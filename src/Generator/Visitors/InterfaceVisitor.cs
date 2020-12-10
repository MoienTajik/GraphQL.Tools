using System.Collections.Generic;
using System.Linq;
using GraphQL.Tools.Generator.Base;
using GraphQL.Tools.Generator.Extensions;
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
                    @interface.Properties
                        .Add(new Property(fieldType.Name,
                            fieldType.GetTypeName(),
                            fieldType.IsArray(),
                            fieldType.IsNullable()));
                }

                @interfaces.Add(@interface);
            }

            return @interfaces;
        }
    }
}