// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.Policies
{
    /// <summary>
    /// An implementation of <see cref="IBuildKeyMappingPolicy"/> that can map
    /// generic types.
    /// </summary>
    public class GenericTypeBuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        private readonly NamedTypeBuildKey _destinationKey;

        /// <summary>
        /// Create a new <see cref="GenericTypeBuildKeyMappingPolicy"/> instance
        /// that will map generic types.
        /// </summary>
        /// <param name="destinationKey">Build key to map to. This must be or contain an open generic type.</param>
        public GenericTypeBuildKeyMappingPolicy(NamedTypeBuildKey destinationKey)
        {
            _destinationKey = destinationKey;
        }

        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping.</param>
        /// <returns>The new build key.</returns>
        public NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context)
        {
            var originalTypeInfo = (buildKey ?? throw new ArgumentNullException(nameof(buildKey))).Type.GetTypeInfo();
            if (originalTypeInfo.IsGenericTypeDefinition)
            {
                // No need to perform a mapping - the source type is an open generic
                return _destinationKey;
            }

            GuardSameNumberOfGenericArguments(originalTypeInfo);
            Type[] genericArguments = originalTypeInfo.GenericTypeArguments;
            Type resultType = _destinationKey.Type.MakeGenericType(genericArguments);
            return new NamedTypeBuildKey(resultType, _destinationKey.Name);
        }

        private void GuardSameNumberOfGenericArguments(TypeInfo sourceTypeInfo)
        {
            if (sourceTypeInfo.GenericTypeArguments.Length != DestinationType.GetTypeInfo().GenericTypeParameters.Length)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Constants.MustHaveSameNumberOfGenericArguments,
                                  sourceTypeInfo.Name, DestinationType.Name),
                    nameof(sourceTypeInfo));
            }
        }

        private Type DestinationType => _destinationKey.Type;
    }
}
