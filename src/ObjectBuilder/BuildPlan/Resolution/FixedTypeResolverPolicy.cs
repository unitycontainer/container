// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.Resolution
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolverPolicy"/> that
    /// calls back into the build chain to build up the dependency, passing
    /// a type given at compile time as its build key.
    /// </summary>
    public class FixedTypeResolverPolicy : IDependencyResolverPolicy
    {
        private readonly NamedTypeBuildKey _keyToBuild;

        /// <summary>
        /// Create a new instance storing the given type.
        /// </summary>
        /// <param name="typeToBuild">Type to resolve.</param>
        public FixedTypeResolverPolicy(Type typeToBuild)
        {
            _keyToBuild = new NamedTypeBuildKey(typeToBuild);
        }

        #region IDependencyResolverPolicy Members

        /// <summary>
        /// Get the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve(IBuilderContext context)
        {
            return (context ?? throw new ArgumentNullException(nameof(context))).NewBuildUp(_keyToBuild);
        }

        #endregion
    }
}
