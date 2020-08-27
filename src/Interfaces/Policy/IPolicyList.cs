using System;

namespace Unity.Policy
{
    /// <summary>
    /// This interface allows manipulation of targeted policies
    /// </summary>
    public interface IPolicyList
    {
        /// <summary>
        /// Get policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="type">Target <see cref="Type"/> this policy applies to</param>
        /// <param name="policy"><see cref="Type"/> of the policy to get</param>
        /// <returns>Instance of the policy or null if none found</returns>
        object? Get(Type? type, Type policy);

        /// <summary>
        /// Set policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="type">Target <see cref="Type"/> this policy applies to</param>
        /// <param name="policy"><see cref="Type"/> of the policy to set</param>
        /// <param name="instance">Instance of the policy to apply</param>
        void Set(Type? type, Type policy, object instance);

        /// <summary>
        /// Remove a policy from the <see cref="Type"/>
        /// </summary>
        /// <param name="type">Target <see cref="Type"/> this policy should be applied to</param>
        /// <param name="policy"><see cref="Type"/> of the policy to clear</param>
        void Clear(Type? type, Type policy);
    }
}
