using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Resolve

        internal object? Resolve(ref PipelineContext context)
        {
            context.Container = this;
            bool? isGeneric = null;
            Contract generic = default;

            do
            {
                // Look for registration
                context.Registration ??= context.Container._scope.Get(in context.Contract, RegistrationCategory.Uninitialized);
                if (null != context.Registration)
                {
                    //Registration found, check value
                    context.Target = Unsafe.As<LifetimeManager>(context.Registration).GetValue(_scope);
                    if (!ReferenceEquals(RegistrationManager.NoValue, context.Target)) return context.Target;
                    else context.Target = null; // TODO: context.Target = null

                    // Resolve from registration
                    ResolveRegistration(ref context);

                    return context.Target;
                }

                // Skip to parent if non generic
                if (!(isGeneric ??= context.Contract.Type.IsGenericType)) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = context.Contract.With(context.Contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (context.Registration = context.Container._scope.Get(in context.Contract, in generic)))
                {
                    // Build from generic factory
                    return GenericRegistration(ref context);
                }
            }
            while (null != (context.Container = context.Container.Parent!));
            context.Container = this;

            // No registration found, resolve unregistered
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context);
        }
        
        #endregion
    }
}
