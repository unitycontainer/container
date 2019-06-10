using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public abstract class MemberPipeline : Pipeline
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

        protected static readonly ConstructorInfo InvalidRegistrationExpressionCtor =
            typeof(InvalidRegistrationException)
                .GetTypeInfo()
                .DeclaredConstructors
                .First(c =>
                {
                    var parameters = c.GetParameters();
                    return 1 == parameters.Length &&
                           typeof(string) == parameters[0].ParameterType;
                });

        protected static readonly Expression NewGuid = Expression.Call(typeof(Guid).GetTypeInfo().GetDeclaredMethod(nameof(Guid.NewGuid)));

        protected static readonly PropertyInfo DataProperty = typeof(Exception).GetTypeInfo().GetDeclaredProperty(nameof(Exception.Data));

        protected static readonly MethodInfo AddMethod = typeof(IDictionary).GetTypeInfo().GetDeclaredMethod(nameof(IDictionary.Add));

        private UnityContainer _container;

        #endregion


        #region Constructor

        public MemberPipeline(UnityContainer container)
        {
            _container = container;
        }

        #endregion


        #region Properties

        public DefaultPolicies Defaults => _container.Defaults;

        #endregion
    }

    public abstract partial class MemberPipeline<TMemberInfo, TData> : MemberPipeline
                                                   where TMemberInfo : MemberInfo
    {
        #region Fields

        protected static readonly IEnumerable<InjectionMember> EmptyCollection = Enumerable.Empty<InjectionMember>();

        #endregion


        #region Constructors

        protected MemberPipeline(UnityContainer container)
            : base(container)
        {
            AttributeFactories = new[]
            {
                new AttributeFactory(typeof(DependencyAttribute),         (a)=>((DependencyResolutionAttribute)a).Name, DependencyResolverFactory),
                new AttributeFactory(typeof(OptionalDependencyAttribute), (a)=>((DependencyResolutionAttribute)a).Name, OptionalDependencyResolverFactory),
            };

            container.Defaults.Set(typeof(MemberSelector<TMemberInfo>), (MemberSelector<TMemberInfo>)Select);
        }

        #endregion


        #region Public Members

        public MemberSelector<TMemberInfo> MemberSelector => Select;

        public void AddFactories(IEnumerable<AttributeFactory> factories)
        {
            AttributeFactories = AttributeFactories.Concat(factories).ToArray();
        }

        public AttributeFactory[] AttributeFactories { get; private set; }

        #endregion


        #region MemberProcessor

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <remarks>
        ///// Call hierarchy:
        ///// <see cref="GetExpressions"/>  
        ///// + <see cref="SelectMembers"/>
        /////   + <see cref="ExpressionsFromSelected"/>
        /////     + <see cref="BuildMemberExpression"/>
        /////       + <see cref="GetResolverExpression"/>
        ///// </remarks>
        ///// <param name="type"></param>
        ///// <param name="registration"></param>
        ///// <returns></returns>
        //public IEnumerable<Expression> GetExpressions(Type type, IRegistration? registration)
        //{
        //    var selector = GetOrDefault(registration);
        //    var members = selector(type, registration?.InjectionMembers);

        //    return ExpressionsFromSelection(type, members);
        //}

        #endregion


        #region Implementation

        protected abstract Type MemberType(TMemberInfo info);

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        protected object? PreProcessResolver(TMemberInfo info, object? resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<PipelineContext>)policy.Resolve;

                case IResolverFactory<Type> factory:
                    return factory.GetResolver<PipelineContext>(MemberType(info));

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

        public virtual MemberSelector<TMemberInfo> GetOrDefault(IPolicySet? registration)
        {
            return (MemberSelector<TMemberInfo>)(registration?.Get(typeof(MemberSelector<TMemberInfo>)) ??
                                                     Defaults.Get(typeof(MemberSelector<TMemberInfo>)) ??
                                                     throw new InvalidOperationException("Should never be null"));
        }

        #endregion


        #region Attribute Resolver Factories

        protected virtual ResolveDelegate<PipelineContext> DependencyResolverFactory(Attribute attribute, object info, object? value = null)
        {
            var type = MemberType((TMemberInfo)info);
            return (ref PipelineContext context) => context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name);
        }

        protected virtual ResolveDelegate<PipelineContext> OptionalDependencyResolverFactory(Attribute attribute, object info, object? value = null)
        {
            var type = MemberType((TMemberInfo)info);
            return (ref PipelineContext context) =>
            {
                try
                {
                    return context.Resolve(type, ((DependencyResolutionAttribute)attribute).Name);
                }
                catch (Exception ex) when (!(ex is CircularDependencyException))
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
            public Func<Attribute, object, object?, ResolveDelegate<PipelineContext>> Factory;
            public Func<Attribute, string> Name;

            public AttributeFactory(Type type, Func<Attribute, string> getName, Func<Attribute, object, object?, ResolveDelegate<PipelineContext>> factory)
            {
                Type = type;
                Name = getName;
                Factory = factory;
            }
        }

        #endregion
    }
}
