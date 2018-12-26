using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public delegate Expression MemberExpressionFactory(Attribute attribute, Expression member, object info, Type type, string name, object resolver);

    public abstract class MemberBuildProcessor
    {
        public abstract IEnumerable<Expression> GetEnumerator(ref BuilderContext context);
    }


    public abstract partial class MemberBuildProcessor<TMemberInfo, TData> : MemberBuildProcessor
                                                where TMemberInfo : MemberInfo
    {
        #region Fields

        protected static readonly MethodInfo StringFormat =
            typeof(string).GetTypeInfo()
                          .DeclaredMethods
                          .First(m =>
                          {
                              var parameters = m.GetParameters();
                              return m.Name == nameof(string.Format) &&
                                     m.GetParameters().Length == 2 &&
                                     typeof(object) == parameters[1].ParameterType;
                          });
        protected static readonly Expression InvalidRegistrationExpression = Expression.New(typeof(InvalidRegistrationException));

        protected (Type type, MemberExpressionFactory factory)[] ResolverFactories;

        #endregion


        #region Constructors

        protected MemberBuildProcessor()
        {
            // Add Unity attribute factories
            ResolverFactories = new (Type type, MemberExpressionFactory factory)[]
            {
                (typeof(DependencyAttribute),         DependencyExpressionFactory),
                (typeof(OptionalDependencyAttribute), OptionalDependencyExpressionFactory),
            };

            // Default expression factory for [Dependency] attribute
            Expression DependencyExpressionFactory(Attribute attribute, Expression member, object memberInfo, Type type, string name, object resolver)
            {
                TMemberInfo info = (TMemberInfo)memberInfo;
                return Expression.Assign(member, ResolveExpression(info, ((DependencyResolutionAttribute)attribute).Name ?? name, resolver));
            }

            // Default expression factory for [OptionalDependency] attribute
            Expression OptionalDependencyExpressionFactory(Attribute attribute, Expression member, object memberInfo, Type type, string name, object resolver)
            {
                TMemberInfo info = (TMemberInfo)memberInfo;
                return Expression.TryCatch(
                            Expression.Assign(member, ResolveExpression(info, ((OptionalDependencyAttribute)attribute).Name ?? name, resolver)),
                        Expression.Catch(typeof(Exception),
                            Expression.Assign(member, Expression.Constant(null, type))));
            }
        }

        protected MemberBuildProcessor((Type type, MemberExpressionFactory factory)[] factories)
        {
            ResolverFactories = factories;
        }

        #endregion


        #region Public Methods

        public virtual void Add(Type type, MemberExpressionFactory factory)
        {
            for (var i = 0; i < ResolverFactories.Length; i++)
            {
                if (ResolverFactories[i].type != type) continue;
                ResolverFactories[i].factory = factory;
                return;
            }

            var factories = new (Type type, MemberExpressionFactory factory)[ResolverFactories.Length + 1];
            Array.Copy(ResolverFactories, factories, ResolverFactories.Length);
            factories[ResolverFactories.Length] = (type, factory);
            ResolverFactories = factories;
        }

        #endregion


        #region Overrides

        /// <inheritdoc />
        public override IEnumerable<Expression> GetEnumerator(ref BuilderContext context)
        {
            var selector = GetPolicy<ISelect<TMemberInfo>>(ref context, context.RegistrationType, context.RegistrationName);
            var members = selector.Select(ref context);
            return GetEnumerator(context.Type, context.Name, context.Variable, members);
        }

        #endregion


        #region Expression Building

        protected virtual IEnumerable<Expression> GetEnumerator(Type type, string name, ParameterExpression variable, IEnumerable<object> members)
        {
            foreach (var member in members)
            {
                MemberExpression memberExpr;

                switch (member)
                {
                    case TMemberInfo memberInfo:
                        memberExpr = CreateMemberExpression(variable, memberInfo);
                        yield return BuildMemberExpression(memberExpr, memberInfo, name, null);
                        break;

                    case InjectionMember<TMemberInfo, TData> injectionMember:
                        var (info, value) = injectionMember.FromType(type);
                        memberExpr = CreateMemberExpression(variable, info);
                        yield return BuildMemberExpression(memberExpr, info, name, value);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown MemberInfo<{typeof(TMemberInfo)}> type");
                }
            }
        }

        protected virtual Expression BuildMemberExpression(MemberExpression member, TMemberInfo info, string name, object resolver)
        {
            foreach (var pair in ResolverFactories)
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                var attribute = info.GetCustomAttributes()
                                    .Where(a => a.GetType()
                                                 .GetTypeInfo()
                                                 .IsAssignableFrom(pair.type.GetTypeInfo()))
                                    .FirstOrDefault();
#else
                var attribute = info.GetCustomAttribute(pair.type);
#endif
                if (null == attribute) continue;
                if (null == pair.factory) break;

                return pair.factory(attribute, member, info, MemberType(info), name, resolver);
            }

            return Expression.Assign(member, ResolveExpression(info, name, resolver));
        }

        #endregion


        #region Implementation

        protected abstract Type MemberType(TMemberInfo info);

        protected virtual Expression ResolveExpression(TMemberInfo info, string name, object resolver)
            => throw new NotImplementedException();

        protected virtual MemberExpression CreateMemberExpression(ParameterExpression variable, TMemberInfo info)
            => throw new NotImplementedException();

        #endregion


        #region Policy Retrieval

        public static TPolicyInterface GetPolicyOrDefault<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)(GetNamedPolicy(ref context, type, name) ??
                                      GetNamedPolicy(ref context, type, string.Empty));

            object GetNamedPolicy(ref BuilderContext c, Type t, string n)
            {
                return (c.Get(t, n, typeof(TPolicyInterface)) ?? (
#if NETCOREAPP1_0 || NETSTANDARD1_0
                    t.GetTypeInfo().IsGenericType
#else
                    t.IsGenericType
#endif
                    ? c.Get(t.GetGenericTypeDefinition(), n, typeof(TPolicyInterface)) ?? c.Get(null, null, typeof(TPolicyInterface))
                    : c.Get(null, null, typeof(TPolicyInterface))));
            }
        }

        public static TPolicyInterface GetPolicy<TPolicyInterface>(ref BuilderContext context, Type type, string name)
        {
            return (TPolicyInterface)
            (context.Get(type, name, typeof(TPolicyInterface)) ?? (
#if NETCOREAPP1_0 || NETSTANDARD1_0
                type.GetTypeInfo().IsGenericType
#else
                type.IsGenericType
#endif
                ? context.Get(type.GetGenericTypeDefinition(), name, typeof(TPolicyInterface)) ?? context.Get(null, null, typeof(TPolicyInterface))
                : context.Get(null, null, typeof(TPolicyInterface))));
        }

        #endregion
    }
}
