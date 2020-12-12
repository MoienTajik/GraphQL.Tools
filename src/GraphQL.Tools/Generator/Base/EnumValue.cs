namespace GraphQL.Tools.Generator.Base
{
    public class EnumValue : IMember
    {
        public EnumValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}