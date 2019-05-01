using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Pipeline
{
    public abstract class MemberBuilder : PipelineBuilder
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

        protected static readonly Expression NewGuid = Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)));

        protected static readonly PropertyInfo DataProperty = typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data));

        protected static readonly MethodInfo AddMethod = typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add));

        private UnityContainer _container;

        #endregion


        #region Constructor

        public MemberBuilder(UnityContainer container)
        {
            _container = container;
        }

        #endregion


        #region Properties

        public DefaultPolicies Defaults => _container.Defaults;

        #endregion
    }

    public abstract partial class MemberBuilder<TMemberInfo, TData> : MemberBuilder,
                                                                      ISelect<TMemberInfo>
                                                  where TMemberInfo : MemberInfo
    {
        #region Fields

        protected static readonly IEnumerable<InjectionMember> EmptyCollection = Enumerable.Empty<InjectionMember>();

        #endregion


        #region Constructors

        protected MemberBuilder(UnityContainer container)
            : base(container)
        {
            AttributeFactories = new[]
            {
                new AttributeFactory(typeof(DependencyAttribute),         (a)=>((DependencyResolutionAttribute)a).Name, DependencyResolverFactory),
                new AttributeFactory(typeof(OptionalDependencyAttribute), (a)=>((DependencyResolutionAttribute)a).Name, OptionalDependencyResolverFactory),
            };
        }

        #endregion


        #region Public Members

        public void AddFactories(IEnumerable<AttributeFactory> factories)
        {
            AttributeFactories = AttributeFactories.Concat(factories).ToArray();
        }

        public AttributeFactory[] AttributeFactories { get; private set; }

        #endregion


        #region MemberProcessor

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
        public IEnumerable<Expression> GetExpressions(Type type, ImplicitRegistration registration)
        {
            var selector = GetOrDefault(registration);
            var members = selector.Select(type, registration);

            return ExpressionsFromSelection(type, members);
        }

        #endregion


        #region ISelect

        public virtual IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            foreach (var injectionMember in ((ImplicitRegistration)registration).InjectionMembers ?? EmptyCollection)
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

        #endregion


        #region Implementation

        protected abstract Type MemberType(TMemberInfo info);

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        protected object? PreProcessResolver(TMemberInfo info, object? resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<BuilderContext>)policy.Resolve;

                case IResolverFactory<Type> factory:
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
#elif NET40
            return info.GetCustomAttributes(true)
                       .Cast<Attribute>()
                       .Where(a => a.GetType()
                                    .GetTypeInfo()
                                    .IsAssignableFrom(type.GetTypeInfo()))
                       .FirstOrDefault();
#else
            return info.GetCustomAttribute(type);
#endif
        }

        public virtual ISelect<TMemberInfo> GetOrDefault(IPolicySet registration)
        {
            return (ISelect<TMemberInfo>)(registration.Get(typeof(ISelect<TMemberInfo>)) ??
                                              Defaults.Get(typeof(ISelect<TMemberInfo>)));
        }

        #endregion


        #region Attribute Resolver Factories

        protected virtual ResolveDelegate<BuilderContext> DependencyResolverFactory(Attribute attribute, object info, object? value = null)
        {
            var type = MemberType((TMemberInfo)info);
            return (ref BuilderContext context) => context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected virtual ResolveDelegate<BuilderContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object? value = null)
        {
            var type = MemberType((TMemberInfo)info);
            return (ref BuilderContext context) =>
            {
                try
                {
                    return context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name);
                }
                catch (Exception ex)
                when (!(ex.InnerException is CircularDependencyException))
                {
                    return value;
                }
            };
        }

        #endregion


        #region Nested Types

        public struct AttributeFactory
        {
            public readonly Type Type;
            public Func<Attribute, object, object?, ResolveDelegate<BuilderContext>> Factory;
            public Func<Attribute, string> Name;

            public AttributeFactory(Type type, Func<Attribute, string> getName, Func<Attribute, object, object?, ResolveDelegate<BuilderContext>> factory)
            {
                Type = type;
                Name = getName;
                Factory = factory;
            }
        }

        #endregion
    }
}
