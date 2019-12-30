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
        #region Fields

        protected static readonly UnaryExpression     ConvertExpression = Expression.Convert(NewGuid, typeof(object));
        protected static readonly ParameterExpression ExceptionExpression = Expression.Variable(typeof(Exception));
        protected static readonly MemberExpression    ExceptionDataExpression = Expression.MakeMemberAccess(ExceptionExpression, DataProperty);

        #endregion


        #region Constructors

        public FieldDiagnostic(IPolicySet policySet) : base(policySet)
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
                    if (injectionMember is InjectionMember<FieldInfo, object> && memberSet.Add(injectionMember))
                        yield return injectionMember;
                }
            }

            // Select Attributed members
            foreach (var member in type.DeclaredFields())
            {
                if (!member.IsDefined(typeof(DependencyResolutionAttribute)) || !memberSet.Add(member))
                    continue;

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
            }
        }

        #endregion


        #region Expression

        protected override Expression GetResolverExpression(FieldInfo info)
        {
            var block = Expression.Block(info.FieldType,
                    Expression.Call(ExceptionDataExpression, AddMethod, ConvertExpression, Expression.Constant(info, typeof(object))),
                Expression.Rethrow(info.FieldType));

            return Expression.TryCatch(base.GetResolverExpression(info),
                   Expression.Catch(ExceptionExpression, block));
        }

        protected override Expression GetResolverExpression(FieldInfo info, object? data)
        {
            var block = Expression.Block(info.FieldType, 
                    Expression.Call(ExceptionDataExpression, AddMethod, ConvertExpression, Expression.Constant(info, typeof(object))),
                Expression.Rethrow(info.FieldType));

            return Expression.TryCatch(base.GetResolverExpression(info, data),
                   Expression.Catch(ExceptionExpression, block));
        }

        #endregion


        #region Resolution

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info)
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

        protected override ResolveDelegate<BuilderContext> GetResolverDelegate(FieldInfo info, object? injector)
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
