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
using Unity.Resolution;

namespace Unity
{
    public partial class MethodDiagnostic : MethodPipeline
    {
        #region Constructors

        public MethodDiagnostic(UnityContainer container) 
            : base(container)
        {
            container.Defaults.Set(typeof(Func<Type, InjectionMember, MethodInfo>), InjectionValidatingSelector);
        }

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            foreach (var injectionMember in ((ImplicitRegistration)registration).InjectionMembers ?? EmptyCollection)
            {
                if (injectionMember is InjectionMember<MethodInfo, object[]> && memberSet.Add(injectionMember))
                    yield return injectionMember;
            }

            // Select Attributed members
            foreach (var member in type.GetDeclaredMethods())
            {
                foreach(var attribute in Markers)
                {
#if NET40
                    if (!member.IsDefined(attribute, true) ||
#else
                    if (!member.IsDefined(attribute) ||
#endif
                        !memberSet.Add(member)) continue;

                    // Validate
                    if (member.IsStatic)
                    {
                        yield return new InvalidRegistrationException(
                            $"Static method {member.Name} on type '{member.DeclaringType.Name}' is marked for injection. Static methods cannot be injected");
                    }

                    if (member.IsPrivate)
                        yield return new InvalidRegistrationException(
                            $"Private method '{member.Name}' on type '{member.DeclaringType.Name}' is marked for injection. Private methods cannot be injected");

                    if (member.IsFamily)
                        yield return new InvalidRegistrationException(
                            $"Protected method '{member.Name}' on type '{member.DeclaringType.Name}' is marked for injection. Protected methods cannot be injected");

                    if (member.IsGenericMethodDefinition)
                    {
                        yield return new InvalidRegistrationException(
                            $"Open generic method {member.Name} on type '{member.DeclaringType.Name}' is marked for injection. Open generic methods cannot be injected.");
                    }

                    var parameters = member.GetParameters();
                    if (parameters.Any(param => param.IsOut))
                    {
                        yield return new InvalidRegistrationException(
                            $"Method {member.Name} on type '{member.DeclaringType.Name}' is marked for injection. Methods with 'out' parameters cannot be injected.");
                    }

                    if (parameters.Any(param => param.ParameterType.IsByRef))
                    {
                        yield return new InvalidRegistrationException(
                            $"Method {member.Name} on type '{member.DeclaringType.Name}' is marked for injection. Methods with 'ref' parameters cannot be injected.");
                    }

                    yield return member;
                    break;
                }
            }
        }

        protected override Expression GetResolverExpression(MethodInfo info, object? resolvers)
        {
            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = Expression.Block(typeof(void),
                Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(info, typeof(object))),
                Expression.Rethrow(typeof(void)));

            return 
                Expression.TryCatch(
                    Expression.Call(
                        Expression.Convert(BuilderContextExpression.Existing, info.DeclaringType),
                        info, CreateDiagnosticParameterExpressions(info.GetParameters(), resolvers)),
                Expression.Catch(ex, block));
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(MethodInfo info, object? resolvers)
        {
            var parameterResolvers = CreateDiagnosticParameterResolvers(info.GetParameters(), resolvers).ToArray();
            return (ref BuilderContext c) =>
            {
                try
                {
                    if (null == c.Existing) return c.Existing;

                    var parameters = new object[parameterResolvers.Length];
                    for (var i = 0; i < parameters.Length; i++)
                        parameters[i] = parameterResolvers[i](ref c);

                    info.Invoke(c.Existing, parameters);
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
    }
}
