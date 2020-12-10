using GraphQL.Tools.Generator.Exceptions;
using System;
using System.Linq;

namespace GraphQL.Tools.Generator.Visitors
{
    public static class VisitorFactory
    {
        /// <summary>
        /// This method creates a single visitor based on the name provided.
        /// </summary>
        /// <param name="visitorName">Visitor name to create.</param>
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

        /// <summary>
        /// This method creates and returns all available visitors in project.
        /// </summary>
        public static IGeneratableTypeVisitor[] CreateAll()
        {
            var visitors = typeof(IGeneratableTypeVisitor)
                .Assembly
                .GetExportedTypes()
                .Where(type => typeof(IGeneratableTypeVisitor).IsAssignableFrom(type))
                .ToArray();

            if (visitors is null || visitors.Any() is false)
                throw new VisitorNotFoundException($"No visitor found!");

            return visitors
                .Select(visitor => (IGeneratableTypeVisitor)Activator.CreateInstance(visitor))
                .ToArray();
        }
    }
}