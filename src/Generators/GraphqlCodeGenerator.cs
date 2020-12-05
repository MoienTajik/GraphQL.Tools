using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotChocolate;

namespace GraphQL.Tools.Generators
{
    [Generator]
    internal class GraphqlCodeGenerator : ISourceGenerator
    {
        private const string @Namespace = "GraphqlCodeGenerator";
        private const string AttributeName = "GenerateGraphqlCodesAttribute";
        private const string AttributeBody = @"
            using System;
            namespace GraphqlCodeGenerator
            {
                [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                public sealed class GenerateGraphqlCodesAttribute : Attribute { }
            }
        ";

        private readonly string _fullAttributeName = $"{@Namespace}.{AttributeName}";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch();
            //if (!context.Compilation.ReferencedAssemblyNames
            //    .Any(ai => ai.Name.Equals("HotChocolate.Types", StringComparison.OrdinalIgnoreCase)))
            //{
            //    throw new InvalidOperationException("HotChocolate.Types is not provided.");
            //}

            // add the attribute text
            context.AddSource(AttributeName, SourceText.From(AttributeBody, Encoding.UTF8));

            // retreive the populated receiver 
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var options = (context.Compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation
                .AddSyntaxTrees(CSharpSyntaxTree
                    .ParseText(SourceText
                        .From(AttributeBody, Encoding.UTF8), options));

            var attributeSymbol = compilation.GetTypeByMetadataName(_fullAttributeName);

            foreach (var candidate in receiver.CandidateClasses)
            {
                var model = compilation.GetSemanticModel(candidate.SyntaxTree);
                var typeSymbol = model.GetDeclaredSymbol(candidate);
                if (typeSymbol != null && typeSymbol.GetAttributes().Any(ad =>
                    ad.AttributeClass != null && ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                {
                    if (attributeSymbol != null)
                    {
                        var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();

                        var classSource = $@"
                            namespace {namespaceName}
                            {{
                                public partial class GraphqlGeneratedCodes
                                {{
                                    {GraphqlQuery.GetQuery()}
                                }}
                            }}
                        ";

                        context.AddSource(
                            $"{typeSymbol.ContainingNamespace.ToDisplayString()}.{typeSymbol.Name}",
                            SourceText.From(classSource, Encoding.UTF8));
                    }
                }
            }
        }
    }

    public static class GraphqlQuery
    {
        public static string GetQuery()
        {
            var schema = SchemaBuilder.New()
                .AddDocumentFromString(
                    @"
                        type Query {
                            hello: String
                    }")
                .AddResolver("Query", "hello", () => "world")
                .Create();



            return "public int Age { get; set; } = 10;";
        }
    }

    internal class SyntaxReceiver : ISyntaxReceiver
    {
        internal List<TypeDeclarationSyntax> CandidateClasses { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
                && classDeclarationSyntax.AttributeLists.Any())
                CandidateClasses.Add(classDeclarationSyntax);
        }
    }
}