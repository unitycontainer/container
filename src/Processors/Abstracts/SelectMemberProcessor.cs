using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class BuildMemberProcessor<TMemberInfo, TData> : ISelect<TMemberInfo>
    {
        #region ISelect

        public IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            return SelectMembers(type, DeclaredMembers(type),
                ((InternalRegistration) registration).InjectionMembers)
                .Distinct();
        }

        #endregion


        #region Implementation

        protected virtual IEnumerable<object> SelectMembers(Type type, IEnumerable<TMemberInfo> members, InjectionMember[] injectors)
        {
            // Select Injected Members
            if (null != injectors)
            {
                foreach (var injectionMember in injectors)
                {
                    if (injectionMember is InjectionMember<TMemberInfo, TData>)
                        yield return injectionMember;
                }
            }

            if (null == members) yield break;

            // Select Attributed members
            foreach (var member in members)
            {
                foreach (var pair in ExpressionFactories)
                {
                    if (!member.IsDefined(pair.type)) continue;

                    yield return member;
                    break;
                }
            }
        }

        protected virtual IEnumerable<TMemberInfo> DeclaredMembers(Type type) => new TMemberInfo[0];

        #endregion
    }
}
