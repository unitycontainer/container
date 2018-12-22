using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity.Policy.Mapping
{
    /// <summary>
    /// An implementation of <see cref="IBuildKeyMappingPolicy"/> that can map
    /// generic types.
    /// </summary>
    public class GenericTypeBuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        #region Fields

        private readonly Type _type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericTypeBuildKeyMappingPolicy"/> instance
        /// that will map generic types.
        /// </summary>
        /// <param name="type">Type mapped to</param>
        /// <param name="name">Name</param>
        public GenericTypeBuildKeyMappingPolicy(Type type, bool build)
        {
            RequireBuild = build;
            _type = type;
        }

        #endregion


        #region IBuildKeyMappingPolicy

        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping.</param>
        /// <returns>The new build key.</returns>
        public Type Map(ref BuilderContext context)
        {
            var targetTypeInfo = context.Type.GetTypeInfo();
            if (targetTypeInfo.IsGenericTypeDefinition)
            {
                // No need to perform a mapping - the source type is an open generic
                return _type;
            }

            if (targetTypeInfo.GenericTypeArguments.Length != _type.GetTypeInfo().GenericTypeParameters.Length)
            {
                // TODO: Add proper error message
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          Constants.MustHaveSameNumberOfGenericArguments,
                                                           context.Type.Name, ""),
                                            nameof(context.Type));
            }

            return MakeGenericTypeOrThrow(targetTypeInfo.GenericTypeArguments);
        }

        /// <summary>
        /// Instructs engine to resolve type rather than build it
        /// </summary>
        public bool RequireBuild { get; } = true;

        private Type MakeGenericTypeOrThrow(Type[] genericArguments)
        {
            try
            {
                return _type.MakeGenericType(genericArguments);
            }
            catch (ArgumentException ae)
            {
                throw new MakeGenericTypeFailedException(ae);
            }
        }

        #endregion
    }
}
