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
        public GenericTypeBuildKeyMappingPolicy(Type type, string name, bool build)
            : base(type, name)
        {
            RequireBuild = build;
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
            var targetTypeInfo = buildKey.Type.GetTypeInfo();
            if (targetTypeInfo.IsGenericTypeDefinition)
            {
                // No need to perform a mapping - the source type is an open generic
                return this;
            }

            if (targetTypeInfo.GenericTypeArguments.Length != Type.GetTypeInfo().GenericTypeParameters.Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          Constants.MustHaveSameNumberOfGenericArguments,
                                                          buildKey.Type.Name, Type.Name),
                                            nameof(buildKey.Type));
            }

            Type[] genericArguments = targetTypeInfo.GenericTypeArguments;
            Type resultType = MakeGenericTypeOrThrow(genericArguments);
            return new NamedTypeBuildKey(resultType, Name);
        }

        /// <summary>
        /// Instructs engine to resolve type rather than build it
        /// </summary>
        public bool RequireBuild { get; } = true;

        private Type MakeGenericTypeOrThrow(Type[] genericArguments)
        {
            try
            {
                return Type.MakeGenericType(genericArguments);
            }
            catch (ArgumentException ae)
            {
                throw new MakeGenericTypeFailedException(ae);
            }
        }

        #endregion
    }
}
