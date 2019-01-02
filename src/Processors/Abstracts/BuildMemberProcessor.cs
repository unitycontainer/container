using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Storage;

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

        public abstract IEnumerable<Expression> GetBuildSteps(Type type, IPolicySet registration);

        public abstract ResolveDelegate<BuilderContext> GetResolver(Type type, IPolicySet registration, ResolveDelegate<BuilderContext> seed);

        #endregion
    }


    public abstract partial class BuildMemberProcessor<TMemberInfo, TData> : BuildMemberProcessor
                                                         where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly IPolicySet _policySet;
        
        #endregion


        #region Constructors

        protected BuildMemberProcessor(IPolicySet policySet)
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

            _policySet = policySet;
        }

        #endregion


        #region Policy Retrieval

        public TPolicyInterface GetPolicy<TPolicyInterface>(IPolicySet registration)
        {
            return (TPolicyInterface)(registration.Get(typeof(TPolicyInterface)) ?? 
                                        _policySet.Get(typeof(TPolicyInterface)));
        }

        #endregion
    }
}
