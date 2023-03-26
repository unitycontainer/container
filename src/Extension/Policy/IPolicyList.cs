using System;

namespace Unity.Extension
{
    /// <summary>
    /// This interface allows manipulation of targeted policies
    /// </summary>
    public interface IPolicyList
    {
        /// <summary>
        /// Get policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="target">Target <see cref="Type"/> this policy applies to</param>
        /// <param name="type">The <see cref="Type"/> of the policy to get</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        object? Get(Type target, Type type);


        /// <summary>
        /// Set policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="target">Target <see cref="Type"/> this policy applies to</param>
        /// <param name="type"><see cref="Type"/> of the policy to set</param>
        /// <param name="policy">The policy to set</param>
        void Set(Type target, Type type, object policy);


        /// <summary>
        /// Remove a policy from the <see cref="Type"/>
        /// </summary>
        /// <param name="type">Target <see cref="Type"/> this policy should be applied to</param>
        /// <param name="policy"><see cref="Type"/> of the policy to clear</param>
        /// <param name="unsubscribe">Indicates if clean should unsubscribe all notification handlers</param>
        void Clear(Type type, Type policy);
    }
}
