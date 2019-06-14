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
        #region Constructors

        public MethodDiagnostic(UnityContainer container) 
            : base(container, new ParametersDiagnosticProcessor())
        {
            container.Defaults.Set(typeof(Func<Type, InjectionMember, MethodInfo>), InjectionValidatingSelector);
        }

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            foreach (var injectionMember in injectionMembers ?? EmptyCollection)
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
                            $"Static method {member.Name} on type '{member.DeclaringType.Name}' is marked for injection. Static methods cannot be injected", member);
                    }

                    if (member.IsPrivate)
                        yield return new InvalidRegistrationException(
                            $"Private method '{member.Name}' on type '{member.DeclaringType.Name}' is marked for injection. Private methods cannot be injected", member);

                    if (member.IsFamily)
                        yield return new InvalidRegistrationException(
                            $"Protected method '{member.Name}' on type '{member.DeclaringType.Name}' is marked for injection. Protected methods cannot be injected", member);

                    if (member.IsGenericMethodDefinition)
                    {
                        yield return new InvalidRegistrationException(
                            $"Open generic method {member.Name} on type '{member.DeclaringType.Name}' is marked for injection. Open generic methods cannot be injected.", member);
                    }

                    yield return member;
                    break;
                }
            }
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<PipelineContext> GetResolverDelegate(MethodInfo info, object? resolvers)
        {
            var resolver = base.GetResolverDelegate(info, resolvers);

            return (ref PipelineContext context) =>
            {
                try
                {
                    // TODO: Add validation 

                    return resolver(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }
            };
        }
        #endregion


        #region Expression 

        protected override Expression GetResolverExpression(MethodInfo info, object? resolvers)
        {
            var block = Expression.Block(typeof(void),
                Expression.Call(ExceptionDataExpr, AddMethodInfo,
                        Expression.Convert(CallNewGuidExpr, typeof(object)),
                        Expression.Constant(info, typeof(object))),
                Expression.Rethrow(typeof(void)));

            return Expression.TryCatch(base.GetResolverExpression(info, resolvers),
                   Expression.Catch(ExceptionExpr, block));

        }

        #endregion
    }
}
