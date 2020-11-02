using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        internal object? Resolve(ref Contract contract, ref PipelineContext parent)
        {
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;
            RegistrationManager? manager;

            do
            {
                // Look for registration
                if (null != (manager = container._scope.Get(in contract)))
                {
                    //Registration found, check value
                    var value = manager.GetValue(container._scope);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    // Resolve from registration
                    return container.ResolveRegistration(ref contract, manager, ref parent);
                }

                // Skip to parent if non generic
                if (!(isGeneric ??= contract.Type.IsGenericType)) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = contract.With(contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = container._scope.Get(in contract, in generic)))
                {
                    // Build from generic factory
                    return container.GenericRegistration(ref contract, manager, ref parent);
                }
            }
            while (null != (container = container.Parent!));

            var child = parent.CreateContext(this, ref contract);

            // No registration found, resolve unregistered
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref generic, ref child)
                : parent.Contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref child)
                    : ResolveUnregistered(ref child);
        }


        internal object? Resolve(ref PipelineContext context)
        {
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;

            do
            {
                // Look for registration
                context.Registration ??= container._scope.Get(in context.Contract, RegistrationCategory.Uninitialized);
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
                if (null != (context.Registration = container._scope.Get(in context.Contract, in generic)))
                {
                    // Build from generic factory
                    return GenericRegistration(ref context);
                }
            }
            while (null != (container = container.Parent!));

            // No registration found, resolve unregistered
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context);
        }
    }
}
