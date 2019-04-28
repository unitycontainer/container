using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Composition;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    public partial class UnityContainer
    {
        internal delegate CompositionDelegate CompositionFactoryDelegate(ref CompositionContext context);

        #region Fields

        internal CompositionFactoryDelegate _factory = ResolvedComposition;

        #endregion



        #region Composition

        private object? Compose(Type type, string? name, params ResolverOverride[] overrides)
        {
            var registration = GetRegistration(type, name);

            // Double-check lock
            lock (registration)
            {
                // Make sure build plan was not yet created
                if (null == registration.Factory)
                {
                    // Create build plan
                    var context = new CompositionContext
                    {
                        Type = type,
                        Name = name,
                        Overrides = overrides,
                        Registration = registration,
                        Container = this,
                    };

                    registration.Factory = _factory.Invoke(ref context);
                }
            }

            // Execute the plan
            return registration.Factory.Invoke(this, null, overrides);
        }

        #endregion


        #region Build Plan


        internal static CompositionDelegate CompiledComposition(ref CompositionContext context)
        {
            var expressions = new List<Expression>();
            //var type = context.Type;
            //var registration = context.Registration;

            //foreach (var processor in _processorsChain)
            //{
            //    foreach (var step in processor.GetExpressions(type, registration))
            //        expressions.Add(step);
            //}

            expressions.Add(BuilderContextExpression.Existing);

            var lambda = Expression.Lambda<CompositionDelegate>(
                Expression.Block(expressions), BuilderContextExpression.Context);

            return lambda.Compile();
        }

        internal static CompositionDelegate ResolvedComposition(ref CompositionContext context)
        {
            // Closures
            ResolveDelegate<BuilderContext>? seedMethod = null;

            var type = context.Type;
            var name = context.Name;
            var overrides = context.Overrides;
            var registration = context.Registration;
            var typeMapped = registration is ExplicitRegistration containerRegistration
                ? containerRegistration.Type : context.Type;

            // Build chain
            foreach (var strategy in registration?.BuildChain ?? throw new ArgumentNullException(nameof(registration.BuildChain)))
                seedMethod = strategy.BuildResolver(context.Container, type, registration, seedMethod);

            // Assemble composer
            return (null == seedMethod)
                ? (CompositionDelegate)((c, e, o) => null)
                : ((UnityContainer container, object? existing, ResolverOverride[] overrides) => 
                {
                    var context = new BuilderContext
                    {
                        RegistrationType = type,
                        Name = name,
                        Type = typeMapped,
                        Registration = registration,
                        Lifetime = container.LifetimeContainer,
                        Overrides = null != overrides && 0 == overrides.Length ? null : overrides,

                        List = new PolicyList(),
                        ExecutePlan = container.ContextExecutePlan,
                        ResolvePlan = container.ContextResolvePlan,
                    };

                    return seedMethod(ref context);
                });
        }


        #endregion
    }
}
