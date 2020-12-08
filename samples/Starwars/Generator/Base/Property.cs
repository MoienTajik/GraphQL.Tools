namespace Starwars.Generator.Base
{
    public class Property
    {
        public Property(string type, string name, bool isNullable)
        {
            Type = type;
            Name = name;
            IsNullable = isNullable;
        }

        public string Type { get; }

        public string Name { get; }

        public bool IsNullable { get; set; }

        public override string ToString()
        {
            return $"public {Type}{(IsNullable ? "?" : "")} {Name} {{ get; set; }}";
        }
    }
}