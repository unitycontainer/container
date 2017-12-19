using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;

namespace Unity.Policy.Mapping
{
    /// <summary>
    /// An implementation of <see cref="IBuildKeyMappingPolicy"/> that can map
    /// generic types.
    /// </summary>
    public class GenericTypeBuildKeyMappingPolicy : NamedTypeBase, IBuildKeyMappingPolicy
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericTypeBuildKeyMappingPolicy"/> instance
        /// that will map generic types.
        /// </summary>
        /// <param name="type">Type mapped to</param>
        /// <param name="name">Name</param>
        public GenericTypeBuildKeyMappingPolicy(Type type, string name)
            : base(type, name)
        {
        }

        /// <summary>
        /// Create a new <see cref="GenericTypeBuildKeyMappingPolicy"/> instance
        /// that will map generic types.
        /// </summary>
        /// <param name="destinationKey">Build key to map to. This must be or contain an open generic type.</param>
        public GenericTypeBuildKeyMappingPolicy(INamedType destinationKey)
            : base(destinationKey.Type, destinationKey.Name)
        {
        }

        #endregion


        #region IBuildKeyMappingPolicy

        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping.</param>
        /// <returns>The new build key.</returns>
        public INamedType Map(INamedType buildKey, IBuilderContext context)
        {
            var originalTypeInfo = buildKey.Type.GetTypeInfo();
            if (originalTypeInfo.IsGenericTypeDefinition)
            {
                // No need to perform a mapping - the source type is an open generic
                return this;
            }

            if (buildKey.Type.GenericTypeArguments.Length != Type.GetTypeInfo().GenericTypeParameters.Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          Constants.MustHaveSameNumberOfGenericArguments,
                                                          buildKey.Type.Name, Type.Name),
                                            nameof(buildKey.Type));
            }

            Type[] genericArguments = originalTypeInfo.GenericTypeArguments;
            Type resultType = Type.MakeGenericType(genericArguments);
            return new NamedTypeBuildKey(resultType, Name);
        }

        #endregion
    }
}
