using System;

namespace Unity.Extension
{
    #region Policy Change Handler

    /// <summary>
    /// Represents the method that will handle the PolicyChanged event raised when a policy is changed
    /// </summary>
    /// <param name="target">The target <see cref="Type"/> the policy applies to</param>
    /// <param name="type">The <see cref="Type"/> of the changed policy</param>
    /// <param name="policy">The policy value</param>
    public delegate void PolicyChangeHandler(Type? target, Type type, object? policy);

    #endregion


    /// <summary>
    /// Dynamic policy collection that provides notifications when policies are changed
    /// </summary>
    public interface IPolicies : IPolicySet, IPolicyList
    {
        /// <summary>
        /// Get default policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        object? Get(Type type, PolicyChangeHandler handler);

        /// <summary>
        /// Get policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        object? Get(Type target, Type type, PolicyChangeHandler handler);


        /// <summary>
        /// Set default policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        void Set(Type type, object? policy, PolicyChangeHandler handler);

        /// <summary>
        /// Set policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="target">The <see cref="Type"/> to register policy under</param>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        void Set(Type target, Type type, object? policy, PolicyChangeHandler handler);


        /// <summary>
        /// Compares stored policy with comparand and, if they are equal, replaces it with new policy
        /// </summary>
        /// <param name="target">The <see cref="Type"/> to register policy under</param>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy</param>
        /// <param name="comparand">The value that is compared to current policy</param>
        /// <returns>The original stored policy</returns>
        TPolicy? CompareExchange<TPolicy>(Type? target, TPolicy policy, TPolicy? comparand)
            where TPolicy : class;

        /// <summary>
        /// Compares stored policy with comparand and, if they are equal, replaces it with new policy
        /// </summary>
        /// <param name="target">The <see cref="Type"/> to register policy under</param>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy</param>
        /// <param name="comparand">The value that is compared to current policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The original stored policy</returns>
        TPolicy? CompareExchange<TPolicy>(Type? target, TPolicy policy, TPolicy? comparand, PolicyChangeHandler handler)
            where TPolicy : class;
    }
}
