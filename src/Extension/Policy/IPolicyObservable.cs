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
    public interface IPolicyObservable : IPolicyList
    {
        /// <summary>
        /// Get policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="target">The <see cref="Type"/> the policy was registered under</param>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        /// <returns>The current policy; returns null if policy has not been set</returns>
        object? Get(Type? target, Type type, PolicyChangeHandler handler);


        /// <summary>
        /// Set policy for the <see cref="Type"/>
        /// </summary>
        /// <param name="target">The <see cref="Type"/> to register policy under</param>
        /// <param name="type"><see cref="Type"/> of the policy</param>
        /// <param name="policy">The policy</param>
        /// <param name="handler">Notifications handler that receives change notifications</param>
        void Set(Type? target, Type type, object policy, PolicyChangeHandler handler);
    }
}
