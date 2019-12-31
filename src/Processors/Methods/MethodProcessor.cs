using System;
using System.Collections.Generic;
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
    public class MethodProcessor : ParametersProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(IPolicySet policySet, UnityContainer container)
            : base(policySet, container)
        {
        }

        #endregion


        #region Selection

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type) => UnityDefaults.SupportedMethods(type);

        public override IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != ((InternalRegistration)registration).InjectionMembers)
            {
                foreach (var injectionMember in ((InternalRegistration)registration).InjectionMembers)
                {
                    if (injectionMember is InjectionMember<MethodInfo, object[]> injector && memberSet.Add(injector))
                        yield return injectionMember;
                }
            }

            // Select Attributed members
            foreach (var member in DeclaredMembers(type))
            {
                if (member.IsDefined(typeof(InjectionMethodAttribute)) && memberSet.Add(member))
                    yield return member;
            }
        }

        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info)
        {
            try
            {
                return Expression.Call(
                    Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType), 
                    info, CreateParameterExpressions(info));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidRegistrationException("Invalid Argument", ex);
            }
        }

        protected override Expression GetResolverExpression(MethodInfo info, object? data)
        {
            object[]? injectors = null != data && data is object[] array && 0 != array.Length ? array : null;

            if (null == injectors) return GetResolverExpression(info);

            try
            {
                return Expression.Call(
                    Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                    info, CreateParameterExpressions(info, injectors));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidRegistrationException("Invalid Argument", ex);
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info)
        {
            var resolvers = CreateParameterResolvers(info);

            return (ref BuilderContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                var dependencies = new object?[resolvers.Length];
                for (var i = 0; i < dependencies.Length; i++)
                    dependencies[i] = resolvers[i](ref c);

                info.Invoke(c.Existing, dependencies);

                return c.Existing;
            };
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object? data)
        {
            object[]? injectors = null != data && data is object[] array && 0 != array.Length ? array : null;

            if (null == injectors) return GetResolverDelegate(info);

            var resolvers = CreateParameterResolvers(info, injectors);

            return (ref BuilderContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                var dependencies = new object?[resolvers.Length];
                for (var i = 0; i < dependencies.Length; i++) dependencies[i] = resolvers[i](ref c);

                info.Invoke(c.Existing, dependencies);

                return c.Existing;
            };
        }

        #endregion
    }
}
