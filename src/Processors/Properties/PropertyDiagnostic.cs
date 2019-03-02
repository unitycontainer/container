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
    public class PropertyDiagnostic : PropertyProcessor
    {
        #region Constructors

        public PropertyDiagnostic(IPolicySet policySet) 
            : base(policySet)
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
                    if (injectionMember is InjectionMember<PropertyInfo, object> && memberSet.Add(injectionMember))
                        yield return injectionMember;
                }
            }

            // Select Attributed members
            foreach (var member in type.GetDeclaredProperties())
            {
                for (var i = 0; i < AttributeFactories.Length; i++)
                {
#if NET40
                    if (!member.IsDefined(AttributeFactories[i].Type, true) ||
#else
                    if (!member.IsDefined(AttributeFactories[i].Type) ||
#endif
                        !memberSet.Add(member)) continue;

                    if (!member.CanWrite)
                        throw new InvalidOperationException(
                            $"Readonly property '{member.Name}' on type '{type?.Name}' is marked for injection. Readonly properties cannot be injected");

                    if (0 != member.GetIndexParameters().Length)
                        throw new InvalidOperationException(
                            $"Indexer '{member.Name}' on type '{type?.Name}' is marked for injection. Indexers cannot be injected");

                    var setter = member.GetSetMethod(true);
                    if (setter.IsStatic)
                        throw new InvalidOperationException(
                            $"Static property '{member.Name}' on type '{type?.Name}' is marked for injection. Static properties cannot be injected");

                    if (setter.IsPrivate)
                        throw new InvalidOperationException(
                            $"Private property '{member.Name}' on type '{type?.Name}' is marked for injection. Private properties cannot be injected");

                    if (setter.IsFamily)
                        throw new InvalidOperationException(
                            $"Protected property '{member.Name}' on type '{type?.Name}' is marked for injection. Protected properties cannot be injected");

                    yield return member;
                    break;
                }
            }
        }

        protected override Expression GetResolverExpression(PropertyInfo property, object resolver)
        {
            var ex = Expression.Variable(typeof(Exception));
            var exData = Expression.MakeMemberAccess(ex, DataProperty);
            var block = 
                Expression.Block(property.PropertyType,
                    Expression.Call(exData, AddMethod,
                        Expression.Convert(NewGuid, typeof(object)),
                        Expression.Constant(property, typeof(object))),
                Expression.Rethrow(property.PropertyType));

            return Expression.TryCatch(base.GetResolverExpression(property, resolver),
                   Expression.Catch(ex, block));
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object resolver)
        {
            var value = PreProcessResolver(info, resolver);
            return (ref BuilderContext context) =>
            {
                try
                {
#if NET40
                    info.SetValue(context.Existing, context.Resolve(info, value), null);
#else
                    info.SetValue(context.Existing, context.Resolve(info, value));
#endif
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
