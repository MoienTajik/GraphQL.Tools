using GraphQL.Tools.Generator;
using GraphQL.Tools.Generator.Base;
using GraphQL.Tools.Generator.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphQL.Tools.Generators
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
                                    {graphqlSchemaFilePaths.Select(GraphqlTypeGenerator.Generate)}
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
            var typeExtractors = new List<IGeneratableTypeExtractor>
            {
                new ClassExtractor(),
                new InterfaceExtractor(),
                new EnumExtractor(),
                new UnionExtractor(),
                new ArgumentExtractor()
            };

            var generatableTypeProvider = new GeneratableTypeProvider(typeExtractors);
            List<IGeneratableType> generatableTypes = generatableTypeProvider.FromSchemaFilePath(schemaFilePath);

            IEnumerable<string> generatedTypes = generatableTypes.Select(type => type.ToString());

            return string.Join(Environment.NewLine, generatedTypes);
        }
    }
}