using System;
using System.Linq;
using GraphQL.Tools.Generator.Exceptions;

namespace GraphQL.Tools.Generator.Visitors
{
    public static class VisitorFactory
    {
        public static IGeneratableTypeVisitor Create(string visitorName)
        {
            visitorName = $"{visitorName}Visitor";

            Type? visitorType = typeof(IGeneratableTypeVisitor)
                .Assembly
                .GetExportedTypes()
                .Where(type => typeof(IGeneratableTypeVisitor).IsAssignableFrom(type))
                .Where(visitorType => visitorType.Name.Equals(visitorName, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();

            if (visitorType is null)
                throw new VisitorNotFoundException($"No visitor named {visitorName} found.");

            return (IGeneratableTypeVisitor)Activator.CreateInstance(visitorType);
        }
    }
}