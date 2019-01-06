using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Processors
{
    public abstract class MemberProcessor
    {
        #region Fields

        protected static readonly MethodInfo StringFormat =
            typeof(string).GetTypeInfo()
                .DeclaredMethods
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return m.Name == nameof(string.Format) &&
                           m.GetParameters().Length == 2 &&
                           typeof(object) == parameters[1].ParameterType;
                });

        protected static readonly Expression InvalidRegistrationExpression = Expression.New(typeof(InvalidRegistrationException));

        #endregion


        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Call hierarchy:
        /// <see cref="GetExpressions"/>  
        /// + <see cref="SelectMembers"/>
        ///   + <see cref="ExpressionsFromSelected"/>
        ///     + <see cref="BuildMemberExpression"/>
        ///       + <see cref="GetResolverExpression"/>
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        public abstract IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Call hierarchy:
        /// <see cref="GetResolver"/>
        /// + <see cref="SelectMembers"/>
        ///   + <see cref="ResolversFromSelected"/>
        ///     + <see cref="BuildMemberResolver"/>
        ///       + <see cref="GetResolverDelegate"/>
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="registration"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public abstract ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed);

        #endregion
    }

    public delegate ResolveDelegate<BuilderContext> AttributeResolverFactory(Attribute attribute, object info, object value = null);

    public abstract partial class MemberProcessor<TMemberInfo, TData> : MemberProcessor,
                                                                        ISelect<TMemberInfo>
                                                    where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly IPolicySet _policySet;
        protected AttributeFactoryNode[] AttributeFactories;

        #endregion


        #region Constructors

        protected MemberProcessor(IPolicySet policySet)
        {
            // Add Unity attribute factories
            AttributeFactories = new[]
            {
                new AttributeFactoryNode(typeof(DependencyAttribute),         DependencyResolverFactory),
                new AttributeFactoryNode(typeof(OptionalDependencyAttribute), OptionalDependencyResolverFactory),
            };

            _policySet = policySet;
        }

        protected MemberProcessor(IPolicySet policySet, Type attribute)
        {
            // Add Unity attribute factories
            AttributeFactories = new[]
            {
                new AttributeFactoryNode(attribute,                           null),
                new AttributeFactoryNode(typeof(DependencyAttribute),         DependencyResolverFactory),
                new AttributeFactoryNode(typeof(OptionalDependencyAttribute), OptionalDependencyResolverFactory),
            };
            _policySet = policySet;
        }

        #endregion


        #region Public Methods

        public void Add(Type type, AttributeResolverFactory resolutionFactory)
        {
            for (var i = 0; i < AttributeFactories.Length; i++)
            {
                if (AttributeFactories[i].Type != type) continue;

                AttributeFactories[i].Factory   = resolutionFactory;
                return;
            }

            var factories = new AttributeFactoryNode[AttributeFactories.Length + 1];
            Array.Copy(AttributeFactories, factories, AttributeFactories.Length);
            factories[AttributeFactories.Length] = new AttributeFactoryNode(type, resolutionFactory);
            AttributeFactories = factories;
        }

        #endregion


        #region MemberProcessor

        /// <inheritdoc />
        public override IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration)
        {
            var selector = GetPolicy<ISelect<TMemberInfo>>(registration);
            var members = selector.Select(type, registration);

            return ExpressionsFromSelection(type, members);
        }

        /// <inheritdoc />
        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed)
        {
            var selector = GetPolicy<ISelect<TMemberInfo>>(registration);
            var members = selector.Select(type, registration);
            var resolvers = ResolversFromSelection(type, members).ToArray();

            return (ref BuilderContext c) =>
            {
                if (null == (c.Existing = seed(ref c))) return null;
                foreach (var resolver in resolvers) resolver(ref c);
                return c.Existing;
            };
        }

        #endregion


        #region ISelect

        public virtual IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != ((InternalRegistration)registration).InjectionMembers)
            {
                foreach (var injectionMember in ((InternalRegistration)registration).InjectionMembers)
                {
                    if (injectionMember is InjectionMember<TMemberInfo, TData> && memberSet.Add(injectionMember))
                        yield return injectionMember;
                }
            }

            // Select Attributed members
            IEnumerable<TMemberInfo> members = DeclaredMembers(type);

            if (null == members) yield break;
            foreach (var member in members)
            {
                for (var i = 0; i < AttributeFactories.Length; i++)
                {
                    if (!member.IsDefined(AttributeFactories[i].Type) || 
                        !memberSet.Add(member)) continue;

                    yield return member;
                    break;
                }
            }
        }

        #endregion


        #region Implementation

        protected abstract Type MemberType(TMemberInfo info);

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        protected virtual void ValidateMember(TMemberInfo info) { }

        protected object PreProcessResolver(TMemberInfo info, object resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<BuilderContext>)policy.Resolve;

                case IResolverFactory factory:
                    return factory.GetResolver<BuilderContext>(MemberType(info));

                case Type type:
                    return typeof(Type) == MemberType(info)
                        ? type : (object)info;
            }

            return resolver;
        }

        protected Attribute GetCustomAttribute(TMemberInfo info, Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            return info.GetCustomAttributes()
                       .Where(a => a.GetType()
                                    .GetTypeInfo()
                                    .IsAssignableFrom(type.GetTypeInfo()))
                       .FirstOrDefault();
#else
            return info.GetCustomAttribute(type);
#endif
        }

        public TPolicyInterface GetPolicy<TPolicyInterface>(IPolicySet registration)
        {
            return (TPolicyInterface)(registration.Get(typeof(TPolicyInterface)) ??
                                        _policySet.Get(typeof(TPolicyInterface)));
        }

        #endregion


        #region Parameter Resolver Factories

        protected virtual ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object value = null)
        {
            var type = MemberType((TMemberInfo)info);
            return (ref BuilderContext context) => context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected virtual ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object value = null)
        {
            var type = MemberType((TMemberInfo)info);
            return (ref BuilderContext context) =>
            {
                try { return context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name); }
                catch { return value; }
            };
        }

        #endregion


        #region Nested Types

        public struct AttributeFactoryNode
        {
            public readonly Type Type;
            public AttributeResolverFactory Factory;

            public AttributeFactoryNode(Type type, AttributeResolverFactory factory)
            {
                Type = type;
                Factory = factory;
            }
        }

        #endregion
    }
}
