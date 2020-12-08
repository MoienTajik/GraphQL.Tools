using GraphQL.Tools.Generator;
using GraphQL.Tools.Generator.Base;
using GraphQL.Tools.Generator.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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
            var classSource = $@"
                            #nullable enable annotations
                            namespace GraphQL.Tools
                            {{
                                public partial class Generated
                                {{
                                    {GraphqlTypeGenerator.Generate()}
                                }}
                            }}
                        ";

            context.AddSource(
                $"GraphQL.Tools.g.cs",
                SourceText.From(classSource, Encoding.UTF8));
        }
    }

    public static class GraphqlTypeGenerator
    {
        public static string Generate()
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
            List<IGeneratableType> generatableTypes = generatableTypeProvider.FromSchemaFilePath(@"D:\Alibaba\Zii\galoo\src\GalooBaba\src\Schemas\BabaMock\Sample.gql");

            IEnumerable<string> generatedTypes = generatableTypes.Select(type => type.ToString());

            return string.Join(Environment.NewLine, generatedTypes);
        }
    }
}