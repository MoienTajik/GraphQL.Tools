using System.Collections.Generic;
using GraphQL.Tools.Generator.Visitors;

namespace GraphQL.Tools
{
    public class GenerationContext
    {
        public GenerationContext(string graphqlSchemaFilePath, IEnumerable<IGeneratableTypeVisitor> visitors, IEnumerable<string> additionalNamespaces)
        {
            GraphqlSchemaFilePath = graphqlSchemaFilePath;
            Visitors = visitors;
            AdditionalNamespaces = additionalNamespaces;
        }

        public string GraphqlSchemaFilePath { get; }

        public IEnumerable<IGeneratableTypeVisitor> Visitors { get; }

        public IEnumerable<string> AdditionalNamespaces { get; }
    }
}