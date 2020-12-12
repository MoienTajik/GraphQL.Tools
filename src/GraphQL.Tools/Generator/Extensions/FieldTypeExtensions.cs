using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Tools.Generator.Extensions
{
    public static class FieldTypeExtensions
    {
        public static bool IsArray(this FieldType fieldType)
        {
            var fieldMetaData = fieldType.GetMetadata<GraphQLFieldDefinition>("__AST_MetaField__")!;
            return fieldMetaData.Type!.IsArrayType();
        }

        public static bool IsNullable(this FieldType fieldType)
        {
            var fieldMetaData = fieldType.GetMetadata<GraphQLFieldDefinition>("__AST_MetaField__")!;
            return fieldMetaData.Type!.Kind != ASTNodeKind.NonNullType;
        }

        public static string GetTypeName(this FieldType fieldType)
        {
            return fieldType.ResolvedType.GetNamedType() is GraphQLTypeReference reference
                ? reference.TypeName
                : ((GraphType) fieldType.ResolvedType.GetNamedType()).Name;
        }
    }
}