using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Processors
{
    public class MethodProcessor : ParametersProcessor<MethodInfo>
    {
        #region Selection

        protected override object Select(Type type, InternalRegistration registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != registration.InjectionMembers)
            {
                foreach (var injectionMember in registration.InjectionMembers)
                {
                    if (injectionMember is InjectionMember<MethodInfo, object[]> injector)
                        memberSet.Add(injector);
                }
            }

            // Select Attributed members
            foreach (var member in DeclaredMembers(type))
            {
                if (member.IsDefined(typeof(InjectionMethodAttribute)))
                    memberSet.Add(member);
            }

            return memberSet;
        }

        #endregion


        #region Overrides
        
        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type) => type.SupportedMethods();
        
        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info)
        {
            try
            {
                return Expression.Call(
                    Expression.Convert(BuilderContext.ExistingExpression, info.DeclaringType), 
                    info, ParameterExpressions(info));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidRegistrationException(InvalidArgument, ex);
            }
        }

        protected override Expression GetResolverExpression(MethodInfo info, object? data)
        {
            object[]? injectors = null != data && data is object[] array && 0 != array.Length ? array : null;

            if (null == injectors) return GetResolverExpression(info);

            try
            {
                return Expression.Call(
                    Expression.Convert(BuilderContext.ExistingExpression, info.DeclaringType),
                    info, ParameterExpressions(info, injectors));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidRegistrationException(InvalidArgument, ex);
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info)
        {
            var resolvers = ParameterResolvers(info);

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

            var resolvers = ParameterResolvers(info, injectors);

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
