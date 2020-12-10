using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GraphQL.Tools.Generator.Visitors;
using Microsoft.CodeAnalysis;

namespace GraphQL.Tools
{
    public class GenerationContext
    {
        private GenerationContext(string graphqlSchemaFilePath, IEnumerable<IGeneratableTypeVisitor> visitors, IEnumerable<string> additionalNamespaces)
        {
            GraphqlSchemaFilePath = graphqlSchemaFilePath;
            Visitors = visitors;
            AdditionalNamespaces = additionalNamespaces;
        }

        public string GraphqlSchemaFilePath { get; }

        public IEnumerable<IGeneratableTypeVisitor> Visitors { get; }

        public IEnumerable<string> AdditionalNamespaces { get; }

        public static IEnumerable<GenerationContext> CreateFrom(GeneratorExecutionContext executionContext)
        {
            foreach (AdditionalText file in executionContext.AdditionalFiles)
            {
                var validGraphqlFileExtensions = new string[2] { ".gql", ".graphql" };
                var fileExtension = Path.GetExtension(file.Path);

                if (validGraphqlFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    var visitors = FindVisitors(executionContext, file);
                    var additionalNamespaces = FindAdditionalNamespaces(executionContext, file);

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
                return VisitorFactory.CreateAll();

            var userDefinedVisitors = commaSeparatedVisitors!
                .Trim()
                .Split(',')
                .Select(visitorName => visitorName.Trim());

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
    }
}