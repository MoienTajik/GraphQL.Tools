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
            var generationContexts = GenerationContext.CreateFrom(context);

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

        private static string GenerateGraphQLTypes(GenerationContext generationContext)
        {
            var generatableTypeProvider = new GeneratableTypeProvider(generationContext.Visitors);
            ImmutableHashSet<IGeneratableType> generatableTypes = generatableTypeProvider.FromSchemaFilePath(generationContext.GraphqlSchemaFilePath);

            IEnumerable<string> generatedTypes = generatableTypes.Select(type => type.ToString());

            return string.Join(Environment.NewLine, generatedTypes);
        }
    }
}