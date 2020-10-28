using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

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
                context.Registration = context.Container._scope.Get(in context.Contract);
                if (null != context.Registration)
                {
                    //Registration found, check value
                    var value = Unsafe.As<LifetimeManager>(context.Registration).GetValue(_scope);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    // Resolve from registration
                    return context.Container.ResolveRegistration(ref context);
                }

                // Skip to parent if non generic
                if (!(isGeneric ??= context.Contract.Type.IsGenericType)) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = context.Contract.With(context.Contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (context.Registration = context.Container._scope.Get(in context.Contract, in generic)))
                {
                    // Build from generic factory
                    return context.Container.GenericRegistration(ref context);
                }
            }
            while (null != (context.Container = context.Container.Parent!));

            // No registration found, resolve unregistered
            context.Container = this;
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref context.Contract, ref generic, ref context)
                : context.Contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context.Contract, ref context);
        }
        
        #endregion


        #region Enumerable

        private ResolveDelegate<PipelineContext> ResolveEnumerable(ref Type type)
        {
            var element = type.GenericTypeArguments[0];
            Debug.Assert(null != element);

            var target = ArrayTargetType(element!) ?? element;
            var pipeline = !target.IsGenericType
                    ? EnumerateMethod.MakeGenericMethod(element, target, target)
                                     .CreatePipeline()
                    : EnumerateMethod.MakeGenericMethod(element, target, target.GetGenericTypeDefinition())
                                     .CreatePipeline();

            return _policies.Pipeline(type, pipeline);
        }

        #endregion


        #region ResolveAsync

        /// <summary>
        /// Builds and resolves registered contract
        /// </summary>
        /// <param name="state"><see cref="ResolveContractAsyncState"/> objects holding 
        /// resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveContractAsync(object? state)
        {
            RequestInfoAsync context = (RequestInfoAsync)state!;

            return Task.FromResult<object?>(context.Manager);
        }

        /// <summary>
        /// Builds and resolves unregistered <see cref="Type"/>
        /// </summary>
        /// <param name="state"><see cref="RequestInfoAsync"/> objects holding resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveAsync(object? state)
        {
            RequestInfoAsync context = (RequestInfoAsync)state!;

            return Task.FromResult<object?>(context.Contract.Type);
        }

        #endregion
    }
}
