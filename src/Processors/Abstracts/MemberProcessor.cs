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

namespace Unity.Processors
{
    /// <summary>
    /// Base class of all processors
    /// </summary>
    public abstract class MemberProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        public abstract IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="registration"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public abstract ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext>? seed);
    }


    /// <summary>
    /// Generic member processor base class
    /// </summary>
    /// <typeparam name="TMemberInfo">Type of MemberInfo such as <see cref="FieldInfo"/>,  <see cref="PropertyInfo"/>,  <see cref="ParameterInfo"/></typeparam>
    /// <typeparam name="TData">Format of the data: <see cref="Object"/> or <see cref="Object[]"/></typeparam>
    public abstract partial class MemberProcessor<TMemberInfo, TData> : MemberProcessor
                                                    where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly IPolicySet _policySet;

        #endregion


        #region Constructors

        protected MemberProcessor(IPolicySet policySet)
        {
            _policySet = policySet;
        }

        protected MemberProcessor(IPolicySet policySet, Type attribute)
        {
            _policySet = policySet;
        }

        #endregion


        #region MemberProcessor

        /// <inheritdoc />
        public override IEnumerable<Expression> GetExpressions(Type type, IPolicySet registration)
        {
            foreach (var member in Select(type, registration))
            {
                switch (member)
                {
                    // TMemberInfo
                    case TMemberInfo info:
                        yield return GetResolverExpression(info);
                        break;

                    // Injection Member
                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        yield return GetResolverExpression(injectionMember.MemberInfo(type),
                                                           injectionMember.Data);
                        break;

                    // Unknown
                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        /// <inheritdoc />
        public override ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext>? seed)
        {
            var members = Select(type, registration).ToArray();
            var resolvers = new ResolveDelegate<BuilderContext>[members.Length];

            for (var i = 0; i < resolvers.Length; i++)
            {
                var member = members[i];
                switch (member)
                {
                    // MemberInfo
                    case TMemberInfo info:
                        resolvers[i] = GetResolverDelegate(info);
                        break;

                    // Injection Member
                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        resolvers[i] = GetResolverDelegate(injectionMember.MemberInfo(type),
                                                           injectionMember.Data);
                        break;

                    // Unknown
                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }

            return (ref BuilderContext c) =>
            {
                if (null == (c.Existing = seed?.Invoke(ref c))) return null;
                foreach (var resolver in resolvers) resolver(ref c);
                return c.Existing;
            };
        }

        #endregion


        #region Selection

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
            foreach (var member in DeclaredMembers(type))
            {
                if (member.IsDefined(typeof(DependencyResolutionAttribute), true) && memberSet.Add(member))
                    yield return member;
            }
        }

        #endregion

        #region Implementation

        protected abstract Type MemberType(TMemberInfo info);

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        #endregion


        #region Expression Implementation

        protected virtual Expression GetResolverExpression(TMemberInfo info)
        {
            throw new NotImplementedException();
        }

        protected abstract Expression GetResolverExpression(TMemberInfo info, object? data);

        #endregion


        #region Resolution Implementation

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

        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info)
        { 
            throw new NotImplementedException();
        }

        protected abstract ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object? data);

        #endregion
    }
}
