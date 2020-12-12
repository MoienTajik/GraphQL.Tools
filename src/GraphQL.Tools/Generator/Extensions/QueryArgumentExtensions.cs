using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Tools.Generator.Extensions
{
    public static class QueryArgumentExtensions
    {
        public static bool IsArray(this QueryArgument queryArgument)
        {
            var fieldMetaData = queryArgument.GetMetadata<GraphQLInputValueDefinition>("__AST_MetaField__")!;
            return fieldMetaData.Type!.IsArrayType();
        }

        public static bool IsNullable(this QueryArgument queryArgument)
        {
            var fieldMetaData = queryArgument.GetMetadata<GraphQLInputValueDefinition>("__AST_MetaField__")!;
            return fieldMetaData.Type!.Kind != ASTNodeKind.NonNullType;
        }

        public static string GetTypeName(this QueryArgument queryArgument)
        {
            return queryArgument.ResolvedType.GetNamedType() is GraphQLTypeReference typeReference
                ? typeReference.TypeName
                : ((GraphType)queryArgument.ResolvedType.GetNamedType()).Name;
        }
    }
}