using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public partial class MethodPipeline : MethodBasePipeline<MethodInfo>
    {
        #region Constructors

        public MethodPipeline(UnityContainer container)
            : base(typeof(InjectionMethodAttribute), container)
        {
        }

        #endregion


        #region Overrides

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type) => type.SupportedMethods();

        public override object Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<MethodInfo, object[]>)
                        memberSet.Add(injectionMember);
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


        #region Expression 


        protected override Expression GetResolverExpression(MethodInfo info)
        {
            try
            {
                return Expression.Call(
                    Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType),
                    info, ParameterExpressions(info));
            }
            catch (InvalidRegistrationException reg)
            {
                // Throw if parameters invalid
                return Expression.Throw(Expression.Constant(reg));
            }
            catch (Exception ex)
            {
                // Throw if parameters invalid
                return Expression.Throw(Expression.Constant(new InvalidRegistrationException(ex)));
            }
        }

        protected override Expression GetResolverExpression(MethodInfo info, object? data)
        {
            object[]? injectors = null != data && data is object[] array && 0 != array.Length ? array : null;

            if (null == injectors) return GetResolverExpression(info);

            try
            {
                return Expression.Call(
                    Expression.Convert(PipelineContext.ExistingExpression, info.DeclaringType),
                    info, ParameterExpressions(info, injectors));
            }
            catch (InvalidRegistrationException reg)
            {
                // Throw if parameters invalid
                return Expression.Throw(Expression.Constant(reg));
            }
            catch (Exception ex)
            {
                // Throw if parameters invalid
                return Expression.Throw(Expression.Constant(new InvalidRegistrationException(ex)));
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(MethodInfo info)
        {
            var resolvers = ParameterResolvers(info);

            return (ref PipelineContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                var dependencies = new object?[resolvers.Length];
                for (var i = 0; i < dependencies.Length; i++)
                    dependencies[i] = resolvers[i](ref c);

                info.Invoke(c.Existing, dependencies);

                return c.Existing;
            };
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(MethodInfo info, object? data)
        {
            object[]? injectors = null != data && data is object[] array && 0 != array.Length ? array : null;
            
            if (null == injectors) return GetResolverDelegate(info);

            var resolvers = ParameterResolvers(info, injectors);

            return (ref PipelineContext c) =>
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
