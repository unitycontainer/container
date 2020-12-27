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
}
