using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Processors
{
    public class MethodDiagnostic : MethodProcessor
    {
        #region Fields

        protected static readonly UnaryExpression ReThrowExpression =
            Expression.Rethrow(typeof(void));

        protected static readonly Expression GuidToObjectExpression =
            Expression.Convert(NewGuidExpression, typeof(object));

        #endregion


        #region Constructors

        public MethodDiagnostic(UnityContainer container) 
            : base(container)
        {
        }

        #endregion


        #region Overrides

        protected override object Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != injectionMembers)
            {
                foreach (var injectionMember in injectionMembers)
                {
                    if (injectionMember is InjectionMember<MethodInfo, object[]> && !memberSet.Add(injectionMember))
                        throw new InvalidOperationException($"Method injected more than once '{injectionMember}'");
                }
            }

            // Select Attributed members
            foreach (var member in type.DeclaredMethods())
            {
                if (!member.IsDefined(typeof(InjectionMethodAttribute)) || !memberSet.Add(member)) 
                    continue;

                // Validate
                if (member.IsStatic)
                {
                    throw new ArgumentException(
                        $"Static method {member.Name} on type '{member.DeclaringType?.Name}' is marked for injection. Static methods cannot be injected");
                }

                if (member.IsPrivate)
                    throw new InvalidOperationException(
                        $"Private method '{member.Name}' on type '{member.DeclaringType?.Name}' is marked for injection. Private methods cannot be injected");

                if (member.IsFamily)
                    throw new InvalidOperationException(
                        $"Protected method '{member.Name}' on type '{member.DeclaringType?.Name}' is marked for injection. Protected methods cannot be injected");

                if (member.IsGenericMethodDefinition)
                {
                    throw new ArgumentException(
                        $"Open generic method {member.Name} on type '{member.DeclaringType?.Name}' is marked for injection. Open generic methods cannot be injected.");
                }

                var parameters = member.GetParameters();
                if (parameters.Any(param => param.IsOut))
                {
                    throw new ArgumentException(
                        $"Method {member.Name} on type '{member.DeclaringType?.Name}' is marked for injection. Methods with 'out' parameters cannot be injected.");
                }

                if (parameters.Any(param => param.ParameterType.IsByRef))
                {
                    throw new ArgumentException(
                        $"Method {member.Name} on type '{member.DeclaringType?.Name}' is marked for injection. Methods with 'ref' parameters cannot be injected.");
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

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            var resolver = attribute.GetResolver<BuilderContext>(info);

            return (ref BuilderContext context) => context.ResolveDiagnostic(info, attribute.Name, resolver);
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info, object? data)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            ResolveDelegate<BuilderContext>? resolver = data switch
            {
                IResolve policy                                   => policy.Resolve,
                IResolverFactory<ParameterInfo> propertyFactory   => propertyFactory.GetResolver<BuilderContext>(info),
                IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<BuilderContext>(info.ParameterType),
                Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<BuilderContext>(type),
                _                                                 => null
            };

            if (null == resolver)
                return (ref BuilderContext context) => context.OverrideDiagnostic(info, attribute.Name, data);
            else
                return (ref BuilderContext context) => context.ResolveDiagnostic(info, attribute.Name, resolver);
        }

        #endregion
    }
}
