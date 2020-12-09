using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using GraphQL.Tools.Generator;
using GraphQL.Tools.Generator.Base;
using GraphQL.Tools.Generator.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GraphQL.Tools
{
    [Generator]
    internal class GraphqlCodeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            List<string> graphqlSchemaFilePaths = new();
            foreach (var file in context.AdditionalFiles)
            {
                if (context.AnalyzerConfigOptions.GetOptions(file)
                    .TryGetValue("build_metadata.additionalfiles.GraphQLSchema", out string? isGraphQLSchema) &&
                    isGraphQLSchema.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    var validGraphqlFileExtensions = new string[2] { ".gql", ".graphql" };
                    var fileExtension = Path.GetExtension(file.Path);

                    if (validGraphqlFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                    {
                        graphqlSchemaFilePaths.Add(file.Path);
                    }
                }
            }

            if (graphqlSchemaFilePaths.Any())
            {
                var classSource = $@"
#nullable enable annotations
namespace GraphQL.Tools
{{
    public partial class Generated
    {{
        {string.Join(Environment.NewLine, graphqlSchemaFilePaths.Select(GraphqlTypeGenerator.Generate))}
    }}
}}
";

                context.AddSource(
                    $"GraphQL.Tools.g.cs",
                    SourceText.From(classSource, Encoding.UTF8));
            }
        }
    }

    public static class GraphqlTypeGenerator
    {
        public static string Generate(string schemaFilePath)
        {
            var typeExtractors = new List<IGeneratableTypeVisitor>
            {
                new ClassVisitor(),
                new InterfaceVisitor(),
                new EnumVisitor(),
                new UnionVisitor(),
                new ArgumentVisitor()
            };

            var generatableTypeProvider = new GeneratableTypeProvider(typeExtractors);
            ImmutableHashSet<IGeneratableType> generatableTypes = generatableTypeProvider.FromSchemaFilePath(schemaFilePath);

            IEnumerable<string> generatedTypes = generatableTypes.Select(type => type.ToString());

            return string.Join(Environment.NewLine, generatedTypes);
        }
    }
}