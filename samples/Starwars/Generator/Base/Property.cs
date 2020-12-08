using GraphQL;

namespace Starwars.Generator.Base
{
    public class Property : IMember
    {
        public Property(string name, string type, bool isNullable)
        {
            Name = name.ToPascalCase();
            Type = ConvertGraphqlScalarNameToClrTypeName(type);
            IsNullable = isNullable;
        }

        public string Name { get; }

        public string Type { get; }

        public bool IsNullable { get; set; }

        private static string ConvertGraphqlScalarNameToClrTypeName(string propertyType)
        {
            return propertyType switch
            {
                "Int" => "int",
                "Float" => "float",
                "String" => "string",
                "Boolean" => "bool",
                "ID" => "Guid",
                _ => propertyType
            };
        }

        public override string ToString()
        {
            return $"public {Type}{(IsNullable ? "?" : "")} {Name} {{ get; set; }}";
        }
    }
}