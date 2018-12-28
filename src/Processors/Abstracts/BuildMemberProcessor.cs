using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;

namespace Unity.Processors
{
    public abstract class BuildMemberProcessor 
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

        #endregion


        #region Public Methods

        public abstract IEnumerable<Expression> GetBuildSteps(ref BuilderContext context);

        public abstract ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context, ResolveDelegate<BuilderContext> seed);

        #endregion
    }


    public abstract partial class BuildMemberProcessor<TMemberInfo, TData> : BuildMemberProcessor
                                                         where TMemberInfo : MemberInfo
    {
        #region Constructors

        protected BuildMemberProcessor()
        {
            // Add Unity attribute factories
            ExpressionFactories = new (Type type, MemberExpressionFactory factory)[]
            {
                (typeof(DependencyAttribute),         DependencyExpressionFactory),
                (typeof(OptionalDependencyAttribute), OptionalDependencyExpressionFactory),
            };

            // Add Unity attribute factories
            ResolverFactories = new (Type type, MemberResolverFactory factory)[]
            {
                (typeof(DependencyAttribute),         DependencyResolverFactory),
                (typeof(OptionalDependencyAttribute), OptionalDependencyResolverFactory),
            };
        }

        #endregion


        #region Policy Retrieval

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
