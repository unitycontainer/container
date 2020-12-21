using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        internal object? Resolve(ref PipelineContext context)
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
                        context.Target = value;
                        return value;
                    }

                    return ImportSource.Local == context.Registration.Source
                        ? ResolveRegistered(ref context)
                        : container.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!context.Contract.Type.IsGenericType) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = context.Contract.With(context.Contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (context.Registration = container.Scope.GetBoundGeneric(in context.Contract, in generic)))
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
