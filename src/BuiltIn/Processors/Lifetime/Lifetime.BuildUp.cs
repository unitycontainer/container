using Unity.Container;
using Unity.Lifetime;

namespace Unity.BuiltIn
{
    public partial class LifetimeProcessor
    {
        public override void PreBuildUp(ref ResolveContext context)
        {
            LifetimeManager lifetime = (LifetimeManager)context.Manager!;

            //if (null == lifetime || lifetime is PerResolveLifetimeManager)
            //    lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

            //if (lifetime is SynchronizedLifetimeManager recoveryPolicy)
            //    context.RequiresRecovery = recoveryPolicy;

            var existing = lifetime.GetValue(context.Disposables);
            if (!ReferenceEquals(RegistrationManager.NoValue, existing))
            {
                context.Existing = existing;
            }
        }


        public override void PostBuildUp(ref ResolveContext context)
        {
            LifetimeManager lifetime = (LifetimeManager)context.Manager!;

            //if (null == lifetime || lifetime is PerResolveLifetimeManager)
            //    lifetime = (LifetimeManager?)context.Get(typeof(LifetimeManager));

            if (!ReferenceEquals(RegistrationManager.NoValue, context.Existing))
                lifetime?.SetValue(context.Existing, context.Disposables);
        }
    }
}
