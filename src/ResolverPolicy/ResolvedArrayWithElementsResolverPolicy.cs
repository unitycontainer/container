using System;
using System.Reflection;
using Unity.Build;
using Unity.Policy;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// An implementation of <see cref="IResolverPolicy"/> that resolves to
    /// to an array populated with the values that result from resolving other instances
    /// of <see cref="IResolverPolicy"/>.
    /// </summary>
    public class ResolvedArrayWithElementsResolverPolicy : IResolverPolicy
    {
        private static readonly MethodInfo ResolverMethodInfo
                = typeof(ResolvedArrayWithElementsResolverPolicy)
                    .GetTypeInfo().GetDeclaredMethod(nameof(DoResolve));

        private delegate object ResolverArrayDelegate<TContext>(ref TContext context, IResolverPolicy[] elementPolicies) where TContext : IBuildContext;
        private readonly IResolverPolicy[] _elementPolicies;
        private readonly Type _type;
        private object _value;

        /// <summary>
        /// Create an instance of <see cref="ResolvedArrayWithElementsResolverPolicy"/>
        /// with the given type and a collection of <see cref="IResolverPolicy"/>
        /// instances to use when populating the result.
        /// </summary>
        /// <param name="elementType">The type.</param>
        /// <param name="elementPolicies">The resolver policies to use when populating an array.</param>
        public ResolvedArrayWithElementsResolverPolicy(Type elementType, params IResolverPolicy[] elementPolicies)
        {
            _type = elementType;
            _elementPolicies = elementPolicies;
        }

        /// <summary>
        /// Resolve the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>An array populated with the results of resolving the resolver policies.</returns>
        public object Resolve<TContext>(ref TContext context) 
            where TContext : IBuildContext
        {
            if (null == _value)
            {
                var resolver = (ResolverArrayDelegate<TContext>)ResolverMethodInfo
                    .MakeGenericMethod(typeof(TContext), _type)
                    .CreateDelegate(typeof(ResolverArrayDelegate<TContext>));

                _value = resolver(ref context, _elementPolicies);
            }


            return _value;
        }

        private static object DoResolve<TContext, T>(ref TContext context, IResolverPolicy[] elementPolicies)
            where TContext : IBuildContext
        {
            T[] result = new T[elementPolicies.Length];

            for (int i = 0; i < elementPolicies.Length; i++)
            {
                result[i] = (T)elementPolicies[i].Resolve(ref context);
            }

            return result;
        }
    }
}
