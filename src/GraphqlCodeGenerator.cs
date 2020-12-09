using GraphQL.Tools.Generator;
using GraphQL.Tools.Generator.Base;
using GraphQL.Tools.Generator.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

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
            var generationContexts = GetGenerationContexts(context);

            if (generationContexts.Any())
            {
                var namespaces = string.Join(Environment.NewLine, generationContexts.SelectMany(context => context.AdditionalNamespaces));
                var classBody = string.Join(Environment.NewLine, generationContexts.Select(GenerateGraphQLTypes));

                var classSource = $@"
{namespaces}

#nullable enable annotations
namespace GraphQL.Tools
{{
    public partial class Generated
    {{
        {classBody}
    }}
}}
";

                context.AddSource(
                    $"GraphQL.Tools.g.cs",
                    SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private static IEnumerable<GenerationContext> GetGenerationContexts(GeneratorExecutionContext context)
        {
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                var validGraphqlFileExtensions = new string[2] { ".gql", ".graphql" };
                var fileExtension = Path.GetExtension(file.Path);

                if (validGraphqlFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    var visitors = FindVisitors(context, file);
                    var additionalNamespaces = FindAdditionalNamespaces(context, file);

                    yield return new GenerationContext(file.Path, visitors, additionalNamespaces);
                }
            }
        }

        private static IEnumerable<IGeneratableTypeVisitor> FindVisitors(GeneratorExecutionContext context, AdditionalText additionalText)
        {
            context.AnalyzerConfigOptions
                                .GetOptions(additionalText)
                                .TryGetValue("build_metadata.graphql.visitors", out string? commaSeparatedVisitors);

            if (string.IsNullOrWhiteSpace(commaSeparatedVisitors))
                return new List<IGeneratableTypeVisitor>
                {
                    new ClassVisitor(),
                    new InterfaceVisitor(),
                    new EnumVisitor(),
                    new UnionVisitor(),
                    new ArgumentVisitor()
                };

            var userDefinedVisitors = commaSeparatedVisitors!.Trim().Split(',');
            return userDefinedVisitors.Select(VisitorFactory.Create);
        }

        private static string[] FindAdditionalNamespaces(GeneratorExecutionContext context, AdditionalText additionalText)
        {
            context.AnalyzerConfigOptions
                .GetOptions(additionalText)
                .TryGetValue("build_metadata.graphql.additionalnamespaces", out string? commaSeparatedAdditionalNamespaces);

            if (commaSeparatedAdditionalNamespaces is null)
                return new string[] { };

            return commaSeparatedAdditionalNamespaces
                .Trim()
                .Split(',')
                .Select(additionalNamespace => $"using {additionalNamespace};")
                .ToArray();
        }

        private static string GenerateGraphQLTypes(GenerationContext generationContext)
        {
            var generatableTypeProvider = new GeneratableTypeProvider(generationContext.Visitors);
            ImmutableHashSet<IGeneratableType> generatableTypes = generatableTypeProvider.FromSchemaFilePath(generationContext.GraphqlSchemaFilePath);

            IEnumerable<string> generatedTypes = generatableTypes.Select(type => type.ToString());

            return string.Join(Environment.NewLine, generatedTypes);
        }
    }
}