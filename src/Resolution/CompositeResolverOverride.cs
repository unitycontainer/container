// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that composites other
    /// ResolverOverride objects. The GetResolver operation then
    /// returns the resolver from the first child override that
    /// matches the current context and request.
    /// </summary>
    public class CompositeResolverOverride : ResolverOverride, IEnumerable<ResolverOverride>
    {
        private readonly List<ResolverOverride> _overrides;

        public CompositeResolverOverride()
            : base(null, null)
        {
            _overrides = new List<ResolverOverride>();
        }

        public CompositeResolverOverride(params ResolverOverride[] resolverOverrides)
            : base(null, null)
        {
            _overrides = null == resolverOverrides || 0 == resolverOverrides.Length 
                ? new List<ResolverOverride>() : new List<ResolverOverride>(resolverOverrides);
        }

        /// <summary>
        /// Add a new <see cref="ResolverOverride"/> to the collection
        /// that is checked.
        /// </summary>
        /// <param name="newOverride">item to add.</param>
        public void Add(ResolverOverride newOverride)
        {
            _overrides.Add(newOverride);
        }

        /// <summary>
        /// Add a set of <see cref="ResolverOverride"/>s to the collection.
        /// </summary>
        /// <param name="newOverrides">items to add.</param>
        public void AddRange(IEnumerable<ResolverOverride> newOverrides)
        {
            _overrides.AddRange(newOverrides);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ResolverOverride> GetEnumerator()
        {
            return _overrides.GetEnumerator();
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            // Walk backwards over the resolvers, this way newer resolvers can replace
            // older ones.
            for (int index = _overrides.Count() - 1; index >= 0; --index)
            {
                var resolver = _overrides[index].GetResolver(context, dependencyType);
                if (resolver != null)
                {
                    return resolver;
                }
            }
            return null;
        }
    }
}
