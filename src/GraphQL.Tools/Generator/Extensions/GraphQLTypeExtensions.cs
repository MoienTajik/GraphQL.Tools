using GraphQLParser.AST;

namespace GraphQL.Tools.Generator.Extensions
{
    public static class GraphQLTypeExtensions
    {
        public static bool IsArrayType(this GraphQLType graphqlType)
        {
            var isArray = false;

            switch (graphqlType.Kind)
            {
                case ASTNodeKind.NonNullType:
                {
                    isArray = IsArrayType(((GraphQLNonNullType)graphqlType).Type!);
                    break;
                }

                case ASTNodeKind.ListType:
                {
                    isArray = true;
                    break;
                }
            }

            return isArray;
        }
    }
}