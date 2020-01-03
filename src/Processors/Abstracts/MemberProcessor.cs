using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
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
        // TODO: Revisit IEnumerable<Expression> vs Expression return type
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
            foreach (var member in (IReadOnlyCollection<object>)Select(type, ((InternalRegistration)registration).InjectionMembers))
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
            var index = 0;
            var members = (IReadOnlyCollection<object>)Select(type, ((InternalRegistration)registration).InjectionMembers);
            var resolvers = new ResolveDelegate<BuilderContext>[members.Count];

            foreach (var member in members)
            {
                switch (member)
                {
                    // MemberInfo
                    case TMemberInfo info:
                        resolvers[index++] = GetResolverDelegate(info);
                        break;

                    // Injection Member
                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        resolvers[index++] = GetResolverDelegate(injectionMember.MemberInfo(type),
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

        protected virtual object Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<TMemberInfo, TData>)
                        memberSet.Add(injectionMember);
                }
            }

            // Select Attributed members
            foreach (var member in DeclaredMembers(type))
            {
                if (member.IsDefined(typeof(DependencyResolutionAttribute), true))
                    memberSet.Add(member);
            }

            return memberSet;
        }

        #endregion

        #region Implementation

        protected virtual Type MemberType(TMemberInfo info) => throw new NotImplementedException();

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        protected virtual ResolveDelegate<BuilderContext>? PreProcessResolver(TMemberInfo info, DependencyResolutionAttribute attribute, object? data) => data switch
        {
            IResolve policy                                 => policy.Resolve,
            IResolverFactory<TMemberInfo> fieldFactory      => fieldFactory.GetResolver<BuilderContext>(info),
            IResolverFactory<Type> typeFactory              => typeFactory.GetResolver<BuilderContext>(MemberType(info)),
            Type type when typeof(Type) != MemberType(info) => attribute.GetResolver<BuilderContext>(type),
            _                                               => null
        };

        #endregion


        #region Expression

        protected virtual Expression GetResolverExpression(TMemberInfo info)
        {
            throw new NotImplementedException();
        }

        protected abstract Expression GetResolverExpression(TMemberInfo info, object? data);

        #endregion


        #region Resolution

        protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info)
        { 
            throw new NotImplementedException();
        }

        protected abstract ResolveDelegate<BuilderContext> GetResolverDelegate(TMemberInfo info, object? data);

        #endregion
    }
}
