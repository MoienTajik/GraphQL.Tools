using System;
using HotChocolate;
using HotChocolate.Execution;

namespace GraphQL.Tools
{
    class Program
    {
        static void Main()
        {
            var schema = SchemaBuilder.New()
                .AddDocumentFromString(
                    @"
                    type Query {
                        hello: String
                    }")
                .AddResolver("Query", "hello", () => "world")
                .Create();

            var executor = schema.MakeExecutable();

            Console.WriteLine(executor.Execute("{ hello }").ToJson());
        }
    }
}