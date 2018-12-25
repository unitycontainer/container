using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Processors
{
    public partial class MemberInfoProcessor<TMemberInfo, TData> : ISelect<TMemberInfo>
                                               where TMemberInfo : MemberInfo
    {
        #region Fields

        private (Type type, Converter<TMemberInfo, object> factory)[] _resolverFactories;

        #endregion


        #region Constructors

        protected MemberInfoProcessor()
        {
            _resolverFactories = new (Type type, Converter<TMemberInfo, object> factory)[]
            {
                (typeof(DependencyAttribute),         info => info),
                (typeof(OptionalDependencyAttribute), info => info),
            };
        }

        protected MemberInfoProcessor((Type type, Converter<TMemberInfo, object> factory)[] factories)
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

        #endregion


        #region ISelect

        public virtual IEnumerable<object> Select(ref BuilderContext context)
        {
            return GetEnumerable(context.Type, DeclaredMembers(context.Type),
                ((InternalRegistration) context.Registration).InjectionMembers);
        }

        #endregion


        #region Implementation

        private IEnumerable<object> GetEnumerable(Type type, TMemberInfo[] members, InjectionMember[] injectors)
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
                    foreach (var pair in _resolverFactories)
                    {
                        if (!member.IsDefined(pair.type)) continue;

                        yield return pair.factory(member);
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
