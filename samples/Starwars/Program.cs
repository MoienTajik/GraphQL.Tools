using System;
using GraphqlCodeGenerator;

namespace Starwars
{
    class Program
    {
        static void Main()
        {
            var gql = new GraphqlGeneratedCodes();
            Console.WriteLine(gql.Age);
        }
    }

    [GenerateGraphqlCodes]
    public partial class GraphqlGeneratedCodes
    {
       
    }
}
