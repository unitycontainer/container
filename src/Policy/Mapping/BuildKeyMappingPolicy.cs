// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;

namespace Unity.Policy.Mapping
{
    /// <summary>
    /// Represents a builder policy for mapping build keys.
    /// </summary>
    public class BuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        private readonly INamedType _newBuildKey;

        /// <summary>
        /// Initialize a new instance of the <see cref="BuildKeyMappingPolicy"/> with the new build key.
        /// </summary>
        /// <param name="newBuildKey">The new build key.</param>
        public BuildKeyMappingPolicy(INamedType newBuildKey)
        {
            _newBuildKey = newBuildKey;
        }

        public BuildKeyMappingPolicy(Type type, string name)
        {
            _newBuildKey = new NamedTypeBuildKey(type, name);
        }

        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping, unused in this implementation.</param>
        /// <returns>The new build key.</returns>
        public INamedType Map(INamedType buildKey, IBuilderContext context)
        {
            return _newBuildKey;
        }
    }
}
