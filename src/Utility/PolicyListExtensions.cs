using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Storage;

// ReSharper disable once CheckNamespace

namespace Unity.Policy
{
    public static class LegacyPolicyListUtilityExtensions
    {
        #region Set

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <param name="policies"></param>
        /// <param name="policyInterface">The interface to register the policy under.</param>
        /// <param name="policy">The default policy to be registered.</param>
        public static void SetDefault(this IPolicyList policies, Type policyInterface, object policy)
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(null, null, policyInterface, policy);
        }

        /// <summary>
        /// Sets a default policy. When checking for a policy, if no specific individual policy
        /// is available, the default will be used.
        /// </summary>
        /// <typeparam name="TPolicyInterface">The interface to register the policy under.</typeparam>
        /// <param name="policies"><see cref="IPolicyList"/> to add the policy to.</param>
        /// <param name="policy">The default policy to be registered.</param>
        public static void SetDefault<TPolicyInterface>(this IPolicyList policies, TPolicyInterface policy)
        {
            (policies ?? throw new ArgumentNullException(nameof(policies))).Set(null, null, typeof(TPolicyInterface), policy);
        }

        #endregion


        #region Get

        /// <summary>
        /// Default resolution/search algorithm for retrieving requested policy
        /// </summary>
        /// <param name="policies"><see cref="IPolicyList"/> to search.</param>
        /// <param name="policyInterface">The interface the policy is registered under.</param>
        /// <param name="buildKey">The key the policy applies.</param>
        /// <returns>The policy in the list, if present; returns null otherwise.</returns>
        public static object GetOrDefault(this IPolicyList policies, Type policyInterface, object buildKey)
        {
            var tuple = ParseBuildKey(buildKey);

            return (buildKey != null ? policies.Get(tuple.Item1, tuple.Item2, policyInterface) : null) ??
                   (tuple.Item1 != null && tuple.Item1.GetTypeInfo().IsGenericType
                       ? Get(policies, policyInterface, ReplaceType(tuple.Item1.GetGenericTypeDefinition())) : null) ??
                   (tuple.Item1 != null ? policies.Get(tuple.Item1, string.Empty, policyInterface) : null) ??
                   (tuple.Item1 != null && tuple.Item1.GetTypeInfo().IsGenericType
                       ? policies.Get(tuple.Item1.GetGenericTypeDefinition(), string.Empty, policyInterface) : null) ??
                   policies.Get(null, null, policyInterface);

            object ReplaceType(Type newType)
            {
                switch (buildKey)
                {
                    case Type _:
                        return newType;

                    case INamedType originalKey:
                        return new NamedTypeBuildKey(newType, originalKey.Name);

                    default:
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            "Cannot extract type from build key {0}.", buildKey), nameof(buildKey));
                }
            }

            object Get(IPolicyList list, Type policy, object key)
            {
                var tupleKey = ParseBuildKey(key);
                return list.Get(tupleKey.Item1, tupleKey.Item2, policy);
            }

            Tuple<Type, string> ParseBuildKey(object key)
            {
                switch (key)
                {
                    case INamedType namedBuildKey:
                        return new Tuple<Type, string>(namedBuildKey.Type, namedBuildKey.Name);

                    case Type typeBuildKey:
                        return new Tuple<Type, string>(typeBuildKey, string.Empty);

                    case string name:
                        return new Tuple<Type, string>(null, name);

                    case null:
                        return new Tuple<Type, string>(null, null);

                    default:
                        return new Tuple<Type, string>(key.GetType(), null);
                }
            }
        }


        #endregion

        internal static TPolicyInterface GetPolicy<TPolicyInterface>(this IPolicyList list, Type type, string name)
        {
            var info = type.GetTypeInfo();

            return (TPolicyInterface)(
                list.Get(type, name, typeof(TPolicyInterface)) ?? (
                    info.IsGenericType ? list.Get(type.GetGenericTypeDefinition(), name, typeof(TPolicyInterface)) ??
                                         list.Get(null, null, typeof(TPolicyInterface))
                        : list.Get(null, null, typeof(TPolicyInterface))));
        }
    }
}
