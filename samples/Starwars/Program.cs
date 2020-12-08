using Starwars.Generator;
using Starwars.Generator.Base;
using Starwars.Generator.Extractors;
using System;
using System.Collections.Generic;

//using GraphqlCodeGenerator;

namespace Starwars
{
    class Program
    {
        static void Main()
        {
            //var gql = new GraphqlGeneratedCodes();
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

            generatableTypes.ForEach(Console.WriteLine);
        }
    }

    //[GenerateGraphqlCodes]
    public partial class GraphqlGeneratedCodes
    {

    }
}
