using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;

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

                    if (ImportSource.Local == context.Registration.Source || ReferenceEquals(this, container))
                        return Policies.ResolveRegistered(ref context);

                    using var scope = context.ScopeTo(container);

                    return Policies.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Fill the Generic Type Definition
                if (context.Generic is null)
                {
                    context.Generic = context.Contract.Type.GetGenericTypeDefinition();
                    generic = context.Contract.With(context.Generic);
                }

                // Check if generic factory is registered
                if (null != (context.Registration = container.Scope.GetBoundGeneric(in context.Contract, in generic)))
                {
                    if (ImportSource.Local == context.Registration.Source || ReferenceEquals(this, container))
                        return ResolveGeneric(generic.Type!, ref context);

                    using var scope = context.ScopeTo(container);
                    
                    return ResolveGeneric(generic.Type!, ref context);
                }
            }
            while (null != (container = container.Parent!));

            var pipeline = Policies.Get<ResolveDelegate<BuilderContext>>(context.Contract.Type);
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
                context.Generic = context.Contract.Type.GetGenericTypeDefinition();
                generic = context.Contract.With(context.Generic);

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

                    if (ImportSource.Local == context.Registration.Source || ReferenceEquals(this, container))
                        return Policies.ResolveRegistered(ref context);

                    using var disposable = context.ScopeTo(container);

                    return Policies.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Check if generic factory is registered
                if (null != (context.Registration = container.Scope.GetBoundGeneric(in context.Contract, in generic)))
                {
                    if (ImportSource.Local == context.Registration.Source || ReferenceEquals(this, container))
                        return ResolveGeneric(generic.Type!, ref context);

                    using var disposable = context.ScopeTo(container);

                    return ResolveGeneric(generic.Type!, ref context);
                }
            }

            var pipeline = Policies.Get<ResolveDelegate<BuilderContext>>(context.Contract.Type);
            if (pipeline is not null) return pipeline(ref context);

            return context.Contract.Type.IsGenericType
                ? UnregisteredGeneric(ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? Policies.ResolveArray(ref context)
                    : Policies.ResolveUnregistered(ref context);
        }

    }
}
