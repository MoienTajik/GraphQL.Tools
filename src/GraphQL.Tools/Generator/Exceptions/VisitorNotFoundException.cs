using System;

namespace GraphQL.Tools.Generator.Exceptions
{
    public class VisitorNotFoundException : Exception
    {
        public VisitorNotFoundException(string message)
            : base(message)
        {
            
        }
    }
}