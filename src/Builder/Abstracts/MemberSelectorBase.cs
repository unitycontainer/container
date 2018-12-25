using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Builder
{
    public abstract partial class MemberSelectorBase<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
    {
        #region Fields

        private (Type type, Converter<TMemberInfo, object> factory)[] _resolverFactories;

        #endregion


        #region Constructors

        protected MemberSelectorBase()
        {
            _resolverFactories = new (Type type, Converter<TMemberInfo, object> factory)[]
            {
                (typeof(DependencyAttribute),         info => info),
                (typeof(OptionalDependencyAttribute), info => info),
            };
        }

        protected MemberSelectorBase((Type type, Converter<TMemberInfo, object> factory)[] factories)
        {
            _resolverFactories = factories ?? throw new ArgumentNullException(nameof(factories));
        }

        #endregion


        #region Public Methods

        public virtual void Add(Type type, Converter<TMemberInfo, object> factory)
        {
            var factories = new (Type type, Converter<TMemberInfo, object> factory)[_resolverFactories.Length + 1];
            Array.Copy(_resolverFactories, factories, _resolverFactories.Length);
            factories[_resolverFactories.Length] = (type, factory);
            _resolverFactories = factories;
        }

        public virtual IEnumerable<object> OnSelect(ref BuilderContext context)
        {
            var members = DeclaredMembers(context.Type);

            return GetInjectionMembers(context.Type, ((InternalRegistration) context.Registration).InjectionMembers)
                     .Concat(GetAttributedMembers(context.Type, members));
        }

        #endregion


        #region Implementation

        protected abstract TMemberInfo[] DeclaredMembers(Type type);

        protected virtual IEnumerable<object> GetInjectionMembers(Type type, InjectionMember[] members)
        {
            if (null == members) yield break;
            foreach (var member in members)
            {
                // TODO: 5.9.0 Filter targeted types
                if (member is InjectionMember<TMemberInfo, TData>)
                    yield return member;
            }
        }

        protected virtual IEnumerable<object> GetAttributedMembers(Type type, TMemberInfo[] members)
        {
            foreach (var member in members)
            {
                foreach (var pair in _resolverFactories)
                {
                    if (!member.IsDefined(pair.type)) continue;

                    yield return pair.factory(member);
                    break;
                }
            }
        }

        #endregion
    }
}
