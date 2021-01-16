using System;
using Unity.Extension;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager : IPolicySet
    {
        /// <inheritdoc />
        public void Clear(Type type) => _policies = null;


        /// <inheritdoc />
        public object? Get(Type type)
        {
            for (var policy = _policies; policy is not null; policy = policy.Next)
            {
                if (policy is PolicyWrapper wrapper)
                {
                    if (ReferenceEquals(wrapper.Item1, type))
                        return wrapper.Item2;
                }
                else
                {
                    if (ReferenceEquals(policy.GetType(), type))
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
