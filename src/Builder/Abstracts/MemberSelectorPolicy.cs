using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Registration;

namespace Unity.Builder
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class MemberSelectorPolicy<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
    {
        #region Public Methods

        public virtual IEnumerable<object> Select<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            return new[]
            {
                GetInjectionMembers(context.Type, ((InternalRegistration)context.Registration).InjectionMembers),
                GetAttributedMembers(context.Type),
                GetDefaultMember(context.Type)
            }
            .SelectMany(o => o)
            .Distinct();
        }

        #endregion


        #region Implementation

        protected virtual TMemberInfo[] DeclaredMembers() => throw new NotImplementedException();

        protected virtual IEnumerable<object> GetInjectionMembers(Type type, InjectionMember[] members)
        {
            if (null == members) yield break;
            foreach (var member in members)
            {
                if (member is InjectionMember<TMemberInfo, TData>)
                    yield return member;
            }
        }

        protected virtual IEnumerable<object> GetAttributedMembers(Type type) =>
            Enumerable.Empty<object>();

        protected virtual IEnumerable<object> GetDefaultMember(Type type) =>
            Enumerable.Empty<object>();


        #endregion
    }
}
