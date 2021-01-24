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
        public void Clear(Type _) => _policies = null;


        /// <inheritdoc />
        public object? Get(Type type)
        {
            for (var policy = _policies; policy is not null; policy = policy.Next as InjectionMethod)
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
                member.Next = _policies;
                _policies = member;
                
                return;
            }
            
            _policies = new PolicyWrapper(type, policy, _policies);
        }

        #endregion


        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var member = _policies;
                     member is not null;
                     member = member.Next as InjectionMember)
            {
                yield return member is PolicyWrapper wrapper
                    ? wrapper.Item2
                    : member;
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
