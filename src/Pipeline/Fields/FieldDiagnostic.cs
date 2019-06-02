using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    public partial class FieldDiagnostic : FieldPipeline
    {
        #region Constructors

        public FieldDiagnostic(UnityContainer container) : base(container)
        {
            container.Defaults.Set(typeof(Func<Type, InjectionMember, FieldInfo>), InjectionValidatingSelector);
        }

        #endregion


        #region Overrides

        public override IEnumerable<object> Select(Type type, InjectionMember[]? injectionMembers)
        {
            HashSet<object> memberSet = new HashSet<object>();

            // Select Injected Members
            foreach (var injectionMember in injectionMembers ?? EmptyCollection)
            {
                if (injectionMember is InjectionMember<FieldInfo, object> && memberSet.Add(injectionMember))
                    yield return injectionMember;
            }

            // Select Attributed members
            foreach (var member in type.GetDeclaredFields())
            {
                foreach(var node in AttributeFactories)
                {
#if NET40
                    if (!member.IsDefined(node.Type, true) ||
#else
                    if (!member.IsDefined(node.Type) ||
#endif
                        !memberSet.Add(member)) continue;

                    if (member.IsStatic)
                    {
                        yield return new InvalidRegistrationException(
                            $"Static field '{member.Name}' on type '{type?.Name}' is marked for injection. Static fields cannot be injected");
                    }

                    if (member.IsInitOnly)
                    {
                        yield return new InvalidRegistrationException(
                            $"Readonly field '{member.Name}' on type '{type?.Name}' is marked for injection. Readonly fields cannot be injected");
                    }

                    if (member.IsPrivate)
                    {
                        yield return new InvalidRegistrationException(
                            $"Private field '{member.Name}' on type '{type?.Name}' is marked for injection. Private fields cannot be injected");
                    }

                    if (member.IsFamily)
                    {
                        yield return new InvalidRegistrationException(
                            $"Protected field '{member.Name}' on type '{type?.Name}' is marked for injection. Protected fields cannot be injected");
                    }

                    yield return member;
                    break;
                }
            }
        }

        protected override Expression GetResolverExpression(FieldInfo field, object? resolver)
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

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object? resolver)
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
