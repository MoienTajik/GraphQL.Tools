using GraphQL.Types;
using Starwars.Generator.Base;
using Starwars.Generator.Extractors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Starwars.Generator
{
    /// <summary>
    /// This class will extract generatable types from GraphQL schema based on specified extractors.
    /// </summary>
    public class GeneratableTypeProvider
    {
        private readonly IEnumerable<IGeneratableTypeExtractor> _generatableTypeExtractors;

        /// <param name="generatableTypeExtractors">The extractors to use for lookup.</param>
        public GeneratableTypeProvider(params IGeneratableTypeExtractor[] generatableTypeExtractors)
            : this(generatableTypeExtractors as IEnumerable<IGeneratableTypeExtractor>)
        {
        }

        /// <param name="generatableTypeExtractors">The extractors to use for lookup.</param>
        public GeneratableTypeProvider(IEnumerable<IGeneratableTypeExtractor> generatableTypeExtractors)
        {
            _generatableTypeExtractors = generatableTypeExtractors;
        }

        /// <summary>
        /// This method extracts generatable types from <paramref name="schema"/>.
        /// </summary>
        /// <param name="schema">GraphQL schema</param>
        /// <returns>Generatable types from ISchema.</returns>
        public List<IGeneratableType> FromSchema(ISchema schema)
        {
            if (schema is null)
                throw new ArgumentNullException(nameof(schema), "Schema could not be null.");

            FieldInfo? field = typeof(Schema).GetField("_additionalInstances", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is null)
                throw new InvalidOperationException($"Could not find `_additionalInstances` field on {nameof(Schema)}.");

            if (field.GetValue(schema) is not List<IGraphType> graphTypes)
                throw new InvalidOperationException($"Get `_additionalInstances` returned invalid type.");

            return _generatableTypeExtractors
                .SelectMany(extractor => extractor.Extract(graphTypes))
                .ToList();
        }

        /// <summary>
        /// This method extracts generatable types from <paramref name="schemaFilePath"/>.
        /// </summary>
        /// <param name="schemaFilePath">GraphQL schema file path</param>
        /// <returns>Generatable types from schema file path.</returns>
        public List<IGeneratableType> FromSchemaFilePath(string schemaFilePath)
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
        /// This method extracts generatable types from <paramref name="schemaFilePath"/>.
        /// </summary>
        /// <param name="schemaFilePath">GraphQL schema file path</param>
        /// <returns>Generatable types from schema file path.</returns>
        public async Task<List<IGeneratableType>> FromSchemaFilePathAsync(string schemaFilePath)
        {
            if (File.Exists(schemaFilePath) is not true)
                throw new FileNotFoundException("No schema with provided path was found.", schemaFilePath);

            var schemaText = await File.ReadAllTextAsync(schemaFilePath);
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
        public List<IGeneratableType> FromSchemaText(string schemaText)
        {
            if (string.IsNullOrWhiteSpace(schemaText))
                throw new ArgumentNullException(nameof(schemaText), "Schema text could not be null or empty.");

            ISchema schema = Schema.For(schemaText);

            return FromSchema(schema);
        }
    }
}
