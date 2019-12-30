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
        #region Fields

        protected static readonly UnaryExpression ConvertExpression        = Expression.Convert(NewGuid, typeof(object));
        protected static readonly ParameterExpression ExceptionExpression  = Expression.Variable(typeof(Exception));
        protected static readonly MemberExpression ExceptionDataExpression = Expression.MakeMemberAccess(ExceptionExpression, DataProperty);

        #endregion


        #region Constructors

        public PropertyDiagnostic(IPolicySet policySet) 
            : base(policySet)
        {
        }

        #endregion


        #region Selection

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
            foreach (var member in type.DeclaredProperties())
            {
                if (!member.IsDefined(typeof(DependencyResolutionAttribute)) || !memberSet.Add(member))
                    continue;

                if (!member.CanWrite)
                    throw new InvalidOperationException(
                        $"Readonly property '{member.Name}' on type '{type?.Name}' is marked for injection. Readonly properties cannot be injected");

                if (0 != member.GetIndexParameters().Length)
                    throw new InvalidOperationException(
                        $"Indexer '{member.Name}' on type '{type?.Name}' is marked for injection. Indexers cannot be injected");

                var setter = member.GetSetMethod(true);

                if (null == setter)
                    throw new InvalidOperationException(
                        $"Readonly property '{member.Name}' on type '{type?.Name}' is marked for injection. Static properties cannot be injected");

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
            }
        }

        #endregion


        #region Expression

        protected override Expression GetResolverExpression(PropertyInfo info)
        {
            var block = Expression.Block(info.PropertyType,
                    Expression.Call(ExceptionDataExpression, AddMethod, ConvertExpression, Expression.Constant(info, typeof(object))),
                Expression.Rethrow(info.PropertyType));

            return Expression.TryCatch(base.GetResolverExpression(info),
                   Expression.Catch(ExceptionExpression, block));
        }


        protected override Expression GetResolverExpression(PropertyInfo info, object? data)
        {
            var block = Expression.Block(info.PropertyType,
                    Expression.Call(ExceptionDataExpression, AddMethod, ConvertExpression, Expression.Constant(info, typeof(object))),
                Expression.Rethrow(info.PropertyType));

            return Expression.TryCatch(base.GetResolverExpression(info, data),
                   Expression.Catch(ExceptionExpression, block));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info)
        {
            var resolver = base.GetResolverDelegate(info);

            return (ref BuilderContext context) =>
            {
                try
                {
                    return resolver(ref context);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(Guid.NewGuid(), info);
                    throw;
                }
            };
        }

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(PropertyInfo info, object? injector)
        {
            var resolver = base.GetResolverDelegate(info, injector);

            return (ref BuilderContext context) =>
            {
                try
                {
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
    }
}
