using System.Collections.Generic;
using System.Linq;
using GraphQL.Tools.Generator.Base;
using GraphQL.Types;

namespace GraphQL.Tools.Generator.Extractors
{
    /// <summary>
    /// This extractor will extract GraphQL unions.
    /// </summary>
    /// <example>
    /// GraphQL schema:
    /// <code>
    /// union Identity = EmailIdentity | PhoneNumberIdentity
    /// 
    /// type EmailIdentity {
    ///     value: String!
    /// }
    /// 
    /// type PhoneNumberIdentity {
    ///     value: Float!
    /// }
    /// </code>
    ///
    /// Extracted interfaces and class:
    /// <code>
    /// public class Identity
    /// {
    /// 	public EmailIdentity? EmailIdentity { get; set; }
    /// 	public PhoneNumberIdentity? PhoneNumberIdentity { get; set; }
    /// }
    /// 
    /// public class EmailIdentity
    /// {
    /// 	public string Value { get; set; }
    /// }
    ///
    /// public class PhoneNumberIdentity
    /// {
    /// 	public float Value { get; set; }
    /// }
    /// </code>
    /// </example>
    public class UnionExtractor : IGeneratableTypeExtractor
    {
        public IEnumerable<IGeneratableType> Extract(IEnumerable<IGraphType> graphTypes)
        {
            foreach (UnionGraphType unionGraphType in graphTypes.Where(type => type is UnionGraphType))
            {
                var @class = new Class(unionGraphType.Name);

                foreach (GraphQLTypeReference typeReference in unionGraphType.PossibleTypes)
                {
                    var propertyName = typeReference.TypeName;
                    var propertyType = typeReference.TypeName;

                    @class.Properties.Add(new Property(propertyName, propertyType, true));
                }

                yield return @class;
            }
        }
    }
}