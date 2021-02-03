using System;
using System.Collections;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager : IEnumerable
    {
        #region PolicySet


        /// <inheritdoc />
        public void Clear(Type _) => Policies = null;

        // TODO: not necessary
        /// <inheritdoc />
        public object? Get(Type type)
        {
            for (var policy = Policies; policy is not null; policy = policy.Next as InjectionMethod)
            {
                if (policy is PolicyWrapper wrapper)
                {
                    if (type.IsAssignableFrom(wrapper.Item1))
                        return wrapper.Item2;
                }
                else
                {
                    if (type.IsAssignableFrom(policy.GetType()))
                        return policy;
                }
            }

            return null;
        }


        /// <inheritdoc />
        public void Set(Type type, object policy)
        {
            if (policy is InjectionMember member && type == policy.GetType())
            {
                member.Next = Policies;
                Policies = member;
                
                return;
            }
            
            Policies = new PolicyWrapper(type, policy, Policies);
        }

        #endregion


        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var member = Policies;
                     member is not null;
                     member = member.Next)
            {
                yield return member;
            }
        }

        #endregion


        #region Nested Types

        private class PolicyWrapper : InjectionMetadata<Type, object?>
        {
            public PolicyWrapper(Type type, object? value, InjectionMember? next)
                : base(type, value)
            {
                Next = next;
            }
        }

        #endregion
    }
}
