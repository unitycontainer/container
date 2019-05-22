using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;
using Unity.Registration;

namespace Unity
{
    public partial class MethodPipeline
    {

        public override IEnumerable<object> Select(Type type, IRegistration? registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            foreach (var injectionMember in registration?.InjectionMembers ?? EmptyCollection)
            {
                if (injectionMember is InjectionMember<MethodInfo, object[]> && memberSet.Add(injectionMember))
                    yield return injectionMember;
            }

            // Select Attributed members
            IEnumerable<MethodInfo> members = DeclaredMembers(type);

            if (null == members) yield break;
            foreach (var member in members)
            {
                foreach (var attribute in Markers)
                {
#if NET40
                    if (!member.IsDefined(attribute, true) ||
#else
                    if (!member.IsDefined(attribute) ||
#endif
                        !memberSet.Add(member)) continue;

                    yield return member;
                    break;
                }
            }
        }
    }
}
