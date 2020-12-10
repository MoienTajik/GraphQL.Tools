using System;

namespace Starwars
{
    class Program
    {
        static void Main()
        {
            var graphqlGenerated = new GraphQL.Tools.Generated.Simple
            {
                Bool = true,
                Int32 = 1,
                Float = 3.5F,
                Double = 1.234F,
                String = "Hello World!"
            };

            Console.WriteLine(@$"{graphqlGenerated.Bool}, {graphqlGenerated.Int32}, {graphqlGenerated.Float}, {graphqlGenerated.Double}, {graphqlGenerated.String}");
        }
    }
}
