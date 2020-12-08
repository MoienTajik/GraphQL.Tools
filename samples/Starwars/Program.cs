using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Starwars.Generator.Base;
//using GraphqlCodeGenerator;

namespace Starwars
{
    class Program
    {
        static void Main()
        {
            //var gql = new GraphqlGeneratedCodes();
            var schemaText = File.ReadAllText(@"D:\Alibaba\Zii\galoo\src\GalooBaba\src\Schemas\BabaMock\Sample.gql");
            var schema = Schema.For(schemaText);

            FieldInfo field = typeof(Schema).GetField("_additionalInstances", BindingFlags.NonPublic | BindingFlags.Instance);
            var registeredTypes = field.GetValue(schema) as List<IGraphType>;

            var generatableTypes = new List<IGeneratableType>();

            // TODO: Convert C# known types to keywords instead
            ExtractClassTypes(registeredTypes, generatableTypes);
            ExtractUnionTypes(registeredTypes, generatableTypes);
            ExtractInterfaceTypes(registeredTypes, generatableTypes);

            generatableTypes.ForEach(Console.WriteLine);
        }

        private static void ExtractClassTypes(ICollection<IGraphType> registeredTypes, ICollection<IGeneratableType> generatableTypes)
        {
            foreach (ObjectGraphType objectGraphType in registeredTypes.Where(type => type is ObjectGraphType))
            {
                var className = objectGraphType.Name;
                var @class = new Class(className);

                ExtractImplementedInterfaces(objectGraphType, @class);

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



                    var isNullable = (fieldType.Metadata.First().Value as GraphQLFieldDefinition).Type.Kind != ASTNodeKind.NonNullType;
                    @class.Properties.Add(new Property(propertyType, propertyName, isNullable));

                    ExtractArgumentTypes(fieldType.Arguments, className, propertyName, generatableTypes);
                }

                generatableTypes.Add(@class);
            }

            static void ExtractImplementedInterfaces(ObjectGraphType objectGraphType, Class @class)
            {
                if (objectGraphType.ResolvedInterfaces.Any())
                {
                    foreach (var interfaceGraphType in objectGraphType.ResolvedInterfaces)
                    {
                        var interfaceName = ((GraphQLTypeReference) interfaceGraphType).TypeName;
                        @class.Interfaces.Add(interfaceName);
                    }
                }
            }
        }

        private static void ExtractArgumentTypes(QueryArguments arguments, string className, string fieldName, ICollection<IGeneratableType> generatableTypes)
        {
            if (arguments.Any())
            {
                var @class = new Class($"{className}_{fieldName}_Arguments");
                foreach (var argument in arguments)
                {
                    var propertyName = argument.Name;

                    var propertyType = "";
                    if (argument.ResolvedType.GetNamedType() is GraphQLTypeReference)
                    {
                        propertyType = ((GraphQLTypeReference)argument.ResolvedType.GetNamedType()).TypeName;
                    }
                    else if (argument.ResolvedType.GetNamedType() is GraphType)
                    {
                        propertyType = ((GraphType)argument.ResolvedType.GetNamedType()).Name;
                    }

                    var isNullable = (argument.Metadata.First().Value as GraphQLInputValueDefinition).Type.Kind != ASTNodeKind.NonNullType;
                    @class.Properties.Add(new Property(propertyType, propertyName, isNullable));
                }

                generatableTypes.Add(@class);
            }
        }

        private static void ExtractUnionTypes(ICollection<IGraphType> registeredTypes, ICollection<IGeneratableType> generatableTypes)
        {
            foreach (UnionGraphType unionGraphType in registeredTypes.Where(type => type is UnionGraphType))
            {
                var @class = new Class(unionGraphType.Name);

                foreach (GraphQLTypeReference typeReference in unionGraphType.PossibleTypes)
                {
                    var propertyName = typeReference.TypeName;
                    var propertyType = typeReference.TypeName;

                    @class.Properties.Add(new Property(propertyType, propertyName, true));
                }

                generatableTypes.Add(@class);
            }
        }

        private static void ExtractInterfaceTypes(ICollection<IGraphType> registeredTypes, ICollection<IGeneratableType> generatableTypes)
        {
            foreach (InterfaceGraphType interfaceGraphType in registeredTypes.Where(type => type is InterfaceGraphType))
            {
                var interfaceName = interfaceGraphType.Name;
                var @interface = new Interface(interfaceName);

                foreach (FieldType fieldType in interfaceGraphType.Fields)
                {
                    var propertyName = fieldType.Name;

                    var propertyType = "";
                    if (fieldType.ResolvedType.GetNamedType() is GraphQLTypeReference)
                    {
                        propertyType = ((GraphQLTypeReference)fieldType.ResolvedType.GetNamedType()).TypeName;
                    }
                    else if (fieldType.ResolvedType.GetNamedType() is GraphType)
                    {
                        propertyType = ((GraphType)fieldType.ResolvedType.GetNamedType()).Name;
                    }

                    var isNullable = (fieldType.Metadata.First().Value as GraphQLFieldDefinition).Type.Kind != ASTNodeKind.NonNullType;
                    @interface.Properties.Add(new Property(propertyType, propertyName, isNullable));

                    ExtractArgumentTypes(fieldType.Arguments, interfaceName, propertyName, generatableTypes);
                }

                generatableTypes.Add(@interface);
            }
        }
    }

    //[GenerateGraphqlCodes]
    public partial class GraphqlGeneratedCodes
    {

    }
}
