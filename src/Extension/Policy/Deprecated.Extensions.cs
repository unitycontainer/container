using System;
using Unity.Policy;

namespace Unity.Extension
{
    /// <summary>
    /// Extension methods to provide convenience overloads
    /// </summary>
    public static class DeprecatedExtensions
    {
        #region Constants

        const string ERROR_NO_LOCAL = "This method is deprecated. Local policies are no longer supported";
        const string ERROR_NO_CASCADING = "This method is deprecated. Cascading policies are no longer supported";
        const string ERROR_NO_LOCAL_CASCADING = "This method is deprecated. Local, cascading policies are no longer supported";

        #endregion


        [Obsolete("This method is deprecated. Use 'Clear<TPolicyInterface>(Type policy)' instead", true)]
        public static void Clear<TPolicyInterface>(this IPolicyList policies, object buildKey)
            => throw new NotImplementedException();

        [Obsolete("This method is deprecated. Use 'Get<TPolicyInterface>(Type policy)' instead", true)]
        public static TPolicyInterface? Get<TPolicyInterface>(this IPolicyList policies, object buildKey)
            => throw new NotImplementedException();

        [Obsolete("This method is deprecated. Use 'Set<TPolicyInterface>(Type policy, TPolicyInterface instance)' instead", true)]
        public static void Set<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy, object buildKey)
            => throw new NotImplementedException();

        [Obsolete("This method is deprecated. Use 'Get(Type? type, Type policy)' instead", true)]
        public static object Get(this IPolicyList policies, Type policyInterface, object buildKey)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_CASCADING, true)]
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_CASCADING, true)]
        public static object? Get(this IPolicyList policies, Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static object? Get(this IPolicyList policies, Type policyInterface, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL, true)]
        public static object? GetNoDefault(this IPolicyList policies, Type policyInterface, object buildKey, bool localOnly)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL_CASCADING, true)]
        public static TPolicyInterface Get<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();

        [Obsolete(ERROR_NO_LOCAL_CASCADING, true)]
        public static TPolicyInterface GetNoDefault<TPolicyInterface>(this IPolicyList policies, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
            => throw new NotImplementedException();
    }
}
