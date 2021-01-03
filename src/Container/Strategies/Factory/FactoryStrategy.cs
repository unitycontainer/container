using System;
using Unity.Extension;

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
                    context.Target = factory(context.Container, context.Type, context.Name, context.Overrides);
            }
            catch (Exception ex)
            {
                context.Capture(ex);
            }
        }
    }
}
