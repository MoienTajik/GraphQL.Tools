using GraphQL.Tools.Generator.Base;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphQL.Tools.Generator.Visitors;

namespace GraphQL.Tools.Generator
{
    /// <summary>
    /// This class will extract generatable types from GraphQL schema based on specified visitors.
    /// </summary>
    public class GeneratableTypeProvider
    {
        private readonly IEnumerable<IGeneratableTypeVisitor> _generatableTypeExtractors;

        /// <param name="generatableTypeExtractors">The visitors to use for lookup.</param>
        public GeneratableTypeProvider(params IGeneratableTypeVisitor[] generatableTypeExtractors)
            : this(generatableTypeExtractors as IEnumerable<IGeneratableTypeVisitor>)
        {
        }

        /// <param name="generatableTypeExtractors">The visitors to use for lookup.</param>
        public GeneratableTypeProvider(IEnumerable<IGeneratableTypeVisitor> generatableTypeExtractors)
        {
            _generatableTypeExtractors = generatableTypeExtractors;
        }

        /// <summary>
        /// This method extracts generatable types from <paramref name="schema"/>.
        /// </summary>
        /// <param name="schema">GraphQL schema</param>
        /// <returns>Generatable types from ISchema.</returns>
        public ImmutableHashSet<IGeneratableType> FromSchema(ISchema schema)
        {
            if (schema is null)
                throw new ArgumentNullException(nameof(schema), "Schema could not be null.");

            FieldInfo? field = typeof(Schema).GetField("_additionalInstances", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is null)
                throw new InvalidOperationException($"Could not find `_additionalInstances` field on {nameof(Schema)}.");

            if (field.GetValue(schema) is not List<IGraphType> graphTypes)
                throw new InvalidOperationException($"Get `_additionalInstances` returned invalid type.");

            return _generatableTypeExtractors
                .SelectMany(visitor => visitor.Visit(graphTypes))
                .ToImmutableHashSet();
        }

        /// <summary>
        /// This method extracts generatable types from <paramref name="schemaFilePath"/>.
        /// </summary>
        /// <param name="schemaFilePath">GraphQL schema file path</param>
        /// <returns>Generatable types from schema file path.</returns>
        public ImmutableHashSet<IGeneratableType> FromSchemaFilePath(string schemaFilePath)
        {
            if (File.Exists(schemaFilePath) is not true)
                throw new FileNotFoundException("No schema with provided path was found.", schemaFilePath);

            var schemaText = File.ReadAllText(schemaFilePath);
            if (string.IsNullOrWhiteSpace(schemaText))
                throw new ArgumentNullException(nameof(schemaText), "Schema text could not be null or empty.");

            ISchema schema = Schema.For(schemaText);

            return FromSchema(schema);
        }

        /// <summary>
        /// This method extracts generatable types from <paramref name="schemaText"/>.
        /// </summary>
        /// <param name="schemaText">GraphQL schema content</param>
        /// <returns>Generatable types from schema text content.</returns>
        public ImmutableHashSet<IGeneratableType> FromSchemaText(string schemaText)
        {
            if (string.IsNullOrWhiteSpace(schemaText))
                throw new ArgumentNullException(nameof(schemaText), "Schema text could not be null or empty.");

            ISchema schema = Schema.For(schemaText);

            return FromSchema(schema);
        }
    }
}
