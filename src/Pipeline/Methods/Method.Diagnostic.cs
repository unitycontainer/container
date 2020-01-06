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
    public partial class MethodDiagnostic : MethodPipeline
    {
        #region Overrides

        public override object Select(ref PipelineBuilder builder)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != builder.InjectionMembers)
            { 
                foreach (var injectionMember in builder.InjectionMembers)
                {
                    if (injectionMember is InjectionMember<MethodInfo, object[]> && !memberSet.Add(injectionMember))
                        return new[] { new InvalidRegistrationException($"Method injected more than once '{injectionMember}'") };
                }
            }

            // Select Attributed members
            foreach (var member in builder.Type.DeclaredMethods())
            {
                if (!member.IsDefined(typeof(InjectionMethodAttribute)) || !memberSet.Add(member))
                    continue;

                // Validate
                if (member.IsStatic)
                {
                    return new[] { new InvalidRegistrationException(
                        $"Static method {member.Name} on type '{member.DeclaringType?.FullName}' can not be invoked. Static methods cannot be called by Unity") };
                }

                if (member.IsPrivate)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Private method '{member.Name}' on type '{member.DeclaringType?.FullName}' can not be invoked. Private methods cannot be called by Unity") };
                }

                if (member.IsFamily)
                { 
                    return new[] { new InvalidRegistrationException(
                        $"Protected method '{member.Name}' on type '{member.DeclaringType?.FullName}' can not be invoked. Protected methods cannot be called by Unity") };
                }

                if (member.IsGenericMethodDefinition)
                {
                    return new[] { new InvalidRegistrationException(
                        $"Open generic method {member.Name} on type '{member.DeclaringType?.FullName}' can not be invoked. Open generic methods cannot be called by Unity.") };
                }
                
                var parameters = member.GetParameters();
                if (parameters.Any(param => param.IsOut))
                {
                    return new[] { new InvalidRegistrationException(
                        $"Method '{member}' on type '{member.DeclaringType?.FullName}' can not be invoked. Methods with 'out' parameters cannot be called by Unity.") };
                }

                if (parameters.Any(param => param.ParameterType.IsByRef))
                {
                    return new[] { new InvalidRegistrationException(
                        $"Method '{member}' on type '{member.DeclaringType?.FullName}' can not be invoked. Methods with 'ref' parameters cannot be called by Unity.") };
                }
            }

            return memberSet;
        }


        #endregion


        #region Expression

        protected override Expression GetResolverExpression(MethodInfo info)
        {
            var block = Expression.Block(typeof(void),
                Expression.Call(ExceptionDataExpression, AddMethodExpression, GuidToObjectExpression, Expression.Constant(info, typeof(object))),
                ReThrowExpression);

            return
                Expression.TryCatch(base.GetResolverExpression(info),
                Expression.Catch(ExceptionVariableExpression, block));
        }

        protected override Expression GetResolverExpression(MethodInfo info, object? data)
        {
            var block = Expression.Block(typeof(void),
                Expression.Call(ExceptionDataExpression, AddMethodExpression, GuidToObjectExpression, Expression.Constant(info, typeof(object))),
                ReThrowExpression);

            return
                Expression.TryCatch(base.GetResolverExpression(info, data),
                Expression.Catch(ExceptionVariableExpression, block));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(MethodInfo info)
        {
            var resolvers = ParameterResolvers(info);

            return (ref PipelineContext c) =>
            {
                if (null == c.Existing) return c.Existing;

                try
                {
                    var dependencies = new object?[resolvers.Length];
                    for (var i = 0; i < dependencies.Length; i++)
                        dependencies[i] = resolvers[i](ref c);

                    info.Invoke(c.Existing, dependencies);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }

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

                try
                {
                    var dependencies = new object?[resolvers.Length];
                    for (var i = 0; i < dependencies.Length; i++) dependencies[i] = resolvers[i](ref c);

                    info.Invoke(c.Existing, dependencies);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }

                return c.Existing;
            };
        }

        #endregion


        #region Parameter Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<PipelineContext>(info);

            return (ref PipelineContext context) => context.ResolveDiagnostic(info, attribute.Name, resolver);
        }

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = PreProcessResolver(info, attribute, data);

            if (null == resolver)
                return (ref PipelineContext context) => context.OverrideDiagnostic(info, attribute.Name, data);
            else
                return (ref PipelineContext context) => context.ResolveDiagnostic(info, attribute.Name, resolver);
        }

        #endregion
    }
}
