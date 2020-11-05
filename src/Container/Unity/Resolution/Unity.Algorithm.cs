using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        internal object? ResolveRequest(ref Contract contract, ref RequestInfo request)
        {
            PipelineContext context;
            RegistrationManager? manager;
            Contract generic = default;

            // Skip to parent if non generic
            if (contract.Type.IsGenericType)
            { 
                // Fill the Generic Type Definition
                generic = contract.With(contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = _scope.GetBoundGeneric(in contract, in generic)))
                {
                    context = new PipelineContext(this, ref contract, manager, ref request);
                    return GenericRegistration(generic.Type!, ref context);
                }
            }

            var container = this;
            while (null != (container = container.Parent!))
            {
                // Try to get registration
                manager = container._scope.Get(in contract);
                if (null != manager)
                {
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(_scope);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    context = new PipelineContext(container, ref contract, manager, ref request);

                    return ImportSource.Local == manager.Source
                        ? ResolveRegistration(ref context)
                        : container.ResolveRegistration(ref context);
                }

                // Skip to parent if non generic
                if (!contract.Type.IsGenericType) continue;

                // Check if generic factory is registered
                if (null != (manager = container._scope.GetBoundGeneric(in contract, in generic)))
                {
                    context = new PipelineContext(container, ref contract, manager, ref request);
                    
                    return ImportSource.Local == manager.Source
                        ? GenericRegistration(generic.Type!, ref context)
                        : container.GenericRegistration(generic.Type!, ref context);
                }
            }

            context = new PipelineContext(this, ref contract, ref request);

            return contract.Type.IsGenericType 
                ? GenericUnregistered(ref generic, ref context)
                : contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context);
        }

        internal object? ResolveContract(ref Contract contract, ref PipelineContext parent)
        {
            var container = this;
            PipelineContext context;
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
                    context = parent.CreateContext(container, ref contract, manager);

                    return ImportSource.Local == manager.Source
                        ? ResolveRegistration(ref context)
                        : container.ResolveRegistration(ref context);
                }

                // Skip to parent if non generic
                if (!contract.Type.IsGenericType) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = contract.With(contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = container._scope.GetBoundGeneric(in contract, in generic)))
                {
                    context = parent.CreateContext(container, ref contract, manager);

                    return ImportSource.Local == manager.Source
                        ? GenericRegistration(generic.Type!, ref context)
                        : container.GenericRegistration(generic.Type!, ref context);
                }
            }
            while (null != (container = container.Parent!));

            context = parent.CreateContext(this, ref contract);

            return context.Contract.Type.IsGenericType
                ? GenericUnregistered(ref generic, ref context)
                : parent.Contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context);
        }


        internal object? Resolve(ref PipelineContext context)
        {
            var container = this;
            Contract generic = default;

            do
            {
                // Try to get registration
                context.Registration ??= container._scope.Get(in context.Contract);
                if (null != context.Registration)
                {
                    context.Target = Unsafe.As<LifetimeManager>(context.Registration).GetValue(_scope);

                    if (!ReferenceEquals(RegistrationManager.NoValue, context.Target)) return context.Target;
                    else context.Target = null; // TODO: context.Target = null

                    return ImportSource.Local == context.Registration.Source
                        ? ResolveRegistration(ref context)
                        : container.ResolveRegistration(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = context.Contract.With(context.Contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (context.Registration = container._scope.GetBoundGeneric(in context.Contract, in generic)))
                {
                    return ImportSource.Local == context.Registration.Source
                        ? GenericRegistration(generic.Type!, ref context)
                        : container.GenericRegistration(generic.Type!, ref context);
                }
            }
            while (null != (container = container.Parent!));

            return context.Contract.Type.IsGenericType 
                ? GenericUnregistered(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context);
        }
    }
}
