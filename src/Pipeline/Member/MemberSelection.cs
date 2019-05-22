using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;
using Unity.Registration;

namespace Unity
{
    public abstract partial class MemberPipeline<TMemberInfo, TData>
    {
        public virtual IEnumerable<object> Select(Type type, IRegistration? registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            foreach (var injectionMember in registration?.InjectionMembers ?? EmptyCollection)
            {
                if (injectionMember is InjectionMember<TMemberInfo, TData> && memberSet.Add(injectionMember))
                    yield return injectionMember;
            }

            // Select Attributed members
            IEnumerable<TMemberInfo> members = DeclaredMembers(type);

            if (null == members) yield break;
            foreach (var member in members)
            {
                foreach (var node in AttributeFactories)
                {
#if NET40
                    if (!member.IsDefined(node.Type, true) ||
#else
                    if (!member.IsDefined(node.Type) ||
#endif
                        !memberSet.Add(member)) continue;

                    yield return member;
                    break;
                }
            }
        }
    }
}
