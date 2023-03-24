using System;
using Unity.Strategies;

namespace Unity.Container
{
    public partial class FactoryStrategy : BuilderStrategy
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            try
            {
                var factory = context.Registration?.Factory;
                if (factory is null)
                    context.Error("Invalid Factory");
                else
                { 
                    if (context.Registration is Lifetime.PerResolveLifetimeManager)
                        context.PerResolve = factory(context.Container, context.Type, context.Name, context.Overrides);
                    else
                        context.Existing = factory(context.Container, context.Type, context.Name, context.Overrides);
                }
            }
            catch (Exception ex)
            {
                context.Capture(ex);
            }
        }
    }
}
