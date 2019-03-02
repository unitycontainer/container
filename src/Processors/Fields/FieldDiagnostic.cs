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
    public class FieldDiagnostic : FieldProcessor
    {
        #region Constructors

        public FieldDiagnostic(IPolicySet policySet) : base(policySet)
        {
        }

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(Type type, IPolicySet registration)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            if (null != ((InternalRegistration)registration).InjectionMembers)
            {
                foreach (var injectionMember in ((InternalRegistration)registration).InjectionMembers)
                {
                    if (injectionMember is InjectionMember<FieldInfo, object> && memberSet.Add(injectionMember))
                        yield return injectionMember;
                }
            }

            // Select Attributed members
            foreach (var member in type.GetDeclaredFields())
            {
                for (var i = 0; i < AttributeFactories.Length; i++)
                {
#if NET40
                    if (!member.IsDefined(AttributeFactories[i].Type, true) ||
#else
                    if (!member.IsDefined(AttributeFactories[i].Type) ||
#endif
                        !memberSet.Add(member)) continue;

                    if (member.IsStatic)
                        throw new InvalidOperationException(
                            $"Static field '{member.Name}' on type '{type?.Name}' is marked for injection. Static fields cannot be injected");

                    if (member.IsInitOnly)
                        throw new InvalidOperationException(
                            $"Readonly field '{member.Name}' on type '{type?.Name}' is marked for injection. Readonly fields cannot be injected");

                    if (member.IsPrivate)
                        throw new InvalidOperationException(
                            $"Private field '{member.Name}' on type '{type?.Name}' is marked for injection. Private fields cannot be injected");

                    if (member.IsFamily)
                        throw new InvalidOperationException(
                            $"Protected field '{member.Name}' on type '{type?.Name}' is marked for injection. Protected fields cannot be injected");

                    yield return member;
                    break;
                }
            }
        }

        protected override Expression GetResolverExpression(FieldInfo field, object resolver)
        {
            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = 
                Expression.Block(field.FieldType,
                    Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(field, typeof(object))),
                Expression.Rethrow(field.FieldType));

            return Expression.TryCatch(base.GetResolverExpression(field, resolver),
                   Expression.Catch(ex, block));
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                try
                {
                    info.SetValue(context.Existing, context.Resolve(info, value));
                    return context.Existing;
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }
            };
        }

        #endregion
    }
}
