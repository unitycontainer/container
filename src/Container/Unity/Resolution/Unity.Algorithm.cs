using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        internal object? Resolve(ref BuilderContext context)
        {
            var container = this;
            Contract generic = default;

            do
            {
                // Try to get registration
                context.Registration ??= container.Scope.Get(in context.Contract);
                if (null != context.Registration)
                {
                    var value = Unsafe.As<LifetimeManager>(context.Registration).GetValue(Scope);
                    if (value.IsValue())
                    {
                        context.Existing = value;
                        return value;
                    }

                    if (context.Registration.IsLocal || ReferenceEquals(this, container))
                        return Policies.ResolveRegistered(ref context);

                    // TODO: Is it required?
                    using var scope = new BuilderContext.ContainerScope(container, ref context);

                    return Policies.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = context.Contract.With(context.TypeDefinition!);

                // Check if generic factory is registered
                if (null != (context.Registration = container.Scope.GetBoundGeneric(in context.Contract, in generic)))
                {
                    if (context.Registration.IsLocal || ReferenceEquals(this, container))
                        return ResolveGeneric(generic.Type!, ref context);
                    
                    // TODO: Is required?
                    using var scope = new BuilderContext.ContainerScope(container, ref context);
                    
                    return ResolveGeneric(generic.Type!, ref context);
                }
            }
            while (null != (container = container.Parent!));

            var pipeline = Policies.Get<ResolverPipeline>(context.Contract.Type);
            if (pipeline is not null) return pipeline(ref context);

            return context.Contract.Type.IsGenericType 
                ? UnregisteredGeneric(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? Policies.ResolveArray(ref context)
                    : Policies.ResolveUnregistered(ref context);
        }


        /// <summary>
        /// Resolve unregistered from Contract
        /// </summary>
        private object? ResolveUnregistered(ref BuilderContext context)
        {
            Contract generic = default;

            // Skip to parent if non generic
            if (context.Contract.Type.IsGenericType)
            {
                // Fill the Generic Type Definition
                generic = context.Contract.With(context.TypeDefinition!);

                // Check if generic factory is registered
                if (null != (context.Registration = Scope.GetBoundGeneric(in context.Contract, in generic)))
                    return ResolveGeneric(generic.Type!, ref context);
            }

            var container = this;
            while (null != (container = container.Parent!))
            {
                // Try to get registration
                context.Registration = container.Scope.Get(in context.Contract);
                if (null != context.Registration)
                {
                    var value = Unsafe.As<LifetimeManager>(context.Registration).GetValue(Scope);
                    if (value.IsValue())
                    {
                        context.Existing = value;
                        return  value;
                    }

                    if (context.Registration.IsLocal || ReferenceEquals(this, container))
                        return Policies.ResolveRegistered(ref context);

                    // TODO: Is required?
                    using var disposable = new BuilderContext.ContainerScope(container, ref context);

                    return Policies.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Check if generic factory is registered
                if (null != (context.Registration = container.Scope.GetBoundGeneric(in context.Contract, in generic)))
                {
                    if (context.Registration.IsLocal || ReferenceEquals(this, container))
                        return ResolveGeneric(generic.Type!, ref context);

                    // TODO: Is required?
                    using var disposable = new BuilderContext.ContainerScope(container, ref context);

                    return ResolveGeneric(generic.Type!, ref context);
                }
            }

            var pipeline = Policies.Get<ResolverPipeline>(context.Contract.Type);
            if (pipeline is not null) return pipeline(ref context);

            return context.Contract.Type.IsGenericType
                ? UnregisteredGeneric(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? Policies.ResolveArray(ref context)
                    : Policies.ResolveUnregistered(ref context);
        }

    }
}
