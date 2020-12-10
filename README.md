# GraphQL.Tools

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)
[![NuGet](https://img.shields.io/nuget/vpre/mediatr.svg)](https://www.nuget.org/packages/graphql.tools)

GraphQL.Tools is a GraphQL to C# compiler which turns your GraphQL schema to a set of C# `classes`, `interfaces`, and `enums`.

## Features 🌀
1. GraphQL `type` to C# `class` compiler
2. GraphQL `interface` to C# `interface` compiler
3. GraphQL `argument` to C# `class` compiler
4. GraphQL `union` to C# `class` compiler
5. GraphQL `enum` to C# `enum` compiler
6. Proper C# `nullable` support

## Getting Started

### 1. Installing GraphQL.Tools
You can install GraphQL.Tools with [NuGet Package Manager Console](https://www.nuget.org/packages/MediatR):

    Install-Package GraphQL.Tools
    
Or via the .NET Core command line interface:

    dotnet add package GraphQL.Tools
    
Either commands, from Package Manager Console or .NET Core CLI, will download and install **GraphQL.Tools** and all required dependencies.

### 2. Defining your GraphQL schema
**\* Important:** GraphQL.Tools just accepts `.gql` and `.graphql` as valid extensions for GraphQL schema files.

Create a new file called `Sample.gql` in the root of your project. This is a simple GraphQL schema that demonstrates most of the GraphQL features which you can access it [here](https://github.com/MoienTajik/GraphQL.Tools/blob/main/samples/Starwars/Sample.gql) from the sample project too:
```graphql
type Query {
    getSimple(name: String!): Simple!
    
    simple: Simple!
    simples: [Simple]!
    nullableSimples: [Simple!]

    identity: Identity!
}

# -----------------------------------------------------------

type Simple {
    bool: Boolean!
    int32: Int!
    float: Float!
    double: Float!
    string: String!
}

# -----------------------------------------------------------

union Identity = EmailIdentity | PhoneNumberIdentity

type EmailIdentity {
    value: String!
}

type PhoneNumberIdentity {
    value: Float!
}

# -----------------------------------------------------------

enum Color {
  RED
  GREEN
  BLUE
}

# -----------------------------------------------------------

interface Character {
  id: Int!
  name: String!
}

type Human implements Character {
  id: Int!
  name: String!
  totalCredits: Int
}
 
type Droid implements Character {
  id: Int!
  name: String!
  primaryFunction: String
}
```

### 3. Configuring GraphQL source generator
Open your project `.csproj` file and add a new `ItemGroup` section to it that contains a `GraphQL` element:
```xml
<ItemGroup>
    <GraphQL Include="$(ProjectDir)Sample.gql" AdditionalNamespaces="System" Visitors="Class, Interface, Enum, Union, Argument" />
</ItemGroup>
```

In this example:
- • `Include` is the absolute path to our previously created GraphQL [schema file](https://github.com/MoienTajik/GraphQL.Tools/blob/main/samples/Starwars/Sample.gql) that ends with `.gql` or `.graphql`.
- • `AdditionalNamespaces` are the comma-separated namespaces that need to be included in the generated file to compile properly (Custom types namespaces like DateTime, TimeSpan, ...).
- • `Visitors` are the comma-separated name of generators (parsers) that should visit the provided schema file. Currently, these are available visitors:
    - 1\. Class
    - 2\. Interface
    - 3\. Enum
    - 4\. Union
    - 5\. Argument

### 4. Done, build the project!
After doing these steps, everything is fine now. Just compile and build the project. The C# file will be generated like this:
```csharp
using System;

#nullable enable annotations
namespace GraphQL.Tools
{
    public partial class Generated
    {
	public class Query
        {
	    public Simple GetSimple { get; set; }
	    public Simple Simple { get; set; }
	    public Simple[] Simples { get; set; }
	    public Simple[]? NullableSimples { get; set; }
	    public Identity Identity { get; set; }
        }

	// -----------------------------------------------------------

	public class Simple
        {
	    public bool Bool { get; set; }
	    public int Int32 { get; set; }
	    public float Float { get; set; }
	    public float Double { get; set; }
	    public string String { get; set; }
        }

	// -----------------------------------------------------------

	public class Identity
        {
	    public EmailIdentity? EmailIdentity { get; set; }
	    public PhoneNumberIdentity? PhoneNumberIdentity { get; set; }
        }

	public class EmailIdentity
        {
	    public string Value { get; set; }
        }

	public class PhoneNumberIdentity
        {
	    public float Value { get; set; }
        }

	// -----------------------------------------------------------

	public enum Color
        {
	    RED,
	    GREEN,
	    BLUE
        }

	// -----------------------------------------------------------

	public interface Character
        {
	    public int Id { get; set; }
	    public string Name { get; set; }
        }

        public class Human : Character
        {
	    public int Id { get; set; }            
	    public string Name { get; set; }
	    public int? TotalCredits { get; set; }
        }

        public class Droid : Character
        {
	    public int Id { get; set; }
	    public string Name { get; set; }
	    public string? PrimaryFunction { get; set; }
        }

	// -----------------------------------------------------------

        public class Query_GetSimple_Arguments
        {
	    public string Name { get; set; }
        }
    }
}
```
**\* Side Note:** You can view the generated file in `$(ProjectDir)\obj\Generated\GraphQL.Tools\GraphQL.Tools.GraphqlCodeGenerator\GraphQL.Tools.g.cs` by adding these elements to the `PropertyGroup` of your `.csproj` file:
```xml
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
<CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
```

### 5. Use generated source in your project
Now you can use the generated file in your project:
```csharp
internal class Program
{
    private static void Main()
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
```

## Give a Star! :star:

If you like or using this project, please give it a star. Thanks!
