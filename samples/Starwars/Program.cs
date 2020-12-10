using System;

namespace Starwars
{
    class Program
    {
        static void Main()
        {
            var simple = new GraphQL.Tools.Generated.Simple
            {
                Bool = true,
                Int32 = 1,
                Float = 3.5F,
                Double = 1.234F,
                String = "Hello World!"
            };

            Console.WriteLine(@$"{simple.Bool}, {simple.Int32}, {simple.Float}, {simple.Double}, {simple.String}");
        }
    }
}
