// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.Policy
{
    /// <summary>
    /// A custom collection over <see cref="IBuilderPolicy"/> objects.
    /// </summary>
    public interface IPolicyList
    {
        /// <summary>
        /// GetOrDefault the non default policy.
        /// </summary>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies to.</param>
        /// <param name="containingPolicyList">The policy list in the chain that the searched for policy was found in, null if the policy was
        /// not found.</param>
        /// <returns>The policy in the list if present; returns null otherwise.</returns>
        IBuilderPolicy Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList);

        /// <summary>
        /// Sets an individual policy.
        /// </summary>
        /// <param name="policyInterface">The <see cref="Type"/> of the policy.</param>
        /// <param name="policy">The policy to be registered.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        void Set(Type policyInterface, IBuilderPolicy policy, object buildKey = null);
        /// <summary>
        /// Removes an individual policy type for a build key.
        /// </summary>
        /// <param name="policyInterface">The type of policy to remove.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        void Clear(Type policyInterface, object buildKey);

        /// <summary>
        /// Removes all policies from the list.
        /// </summary>
        void ClearAll();
    }
}
