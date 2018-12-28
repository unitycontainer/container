using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Processors
{
    public abstract partial class BuildMemberProcessor<TMemberInfo, TData> : ISelect<TMemberInfo>
    {
        #region ISelect

        public virtual IEnumerable<object> Select(ref BuilderContext context)
        {
            return GetEnumerator(context.Type, DeclaredMembers(context.Type),
                ((InternalRegistration) context.Registration).InjectionMembers);
        }

        #endregion


        #region Implementation

        private IEnumerable<object> GetEnumerator(Type type, TMemberInfo[] members, InjectionMember[] injectors)
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

            // Select Attributed members
            if (null != members)
            {
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

            // Select default
            var defaultSelection = GetDefault(type, members);
            if (null != defaultSelection) yield return defaultSelection;
        }

        protected virtual TMemberInfo[] DeclaredMembers(Type type) => new TMemberInfo[0];

        protected virtual object GetDefault(Type type, TMemberInfo[] members) => null;

        #endregion

    }
}
