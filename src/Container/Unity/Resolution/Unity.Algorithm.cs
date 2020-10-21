using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Resolve

        /// <inheritdoc />
//#if NET5_0
//        [SkipLocalsInit]
//#endif
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            var contract = new Contract(type, name);
            var container = this;
            bool? isGeneric = null;
            Contract generic = default;
            RegistrationCategory cutoff = RegistrationCategory.Uninitialized;

            do
            {
                // Look for registration
                var manager = container._scope.Get(in contract, cutoff);
                if (null != manager)
                {
                    //Registration found, check value
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(_scope.Disposables);
                    if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                    // Resolve from registration
                    return container.ResolveRegistration(ref contract, manager, overrides);
                }

                cutoff = RegistrationCategory.Internal;

                // Skip to parent if non generic
                if (!(isGeneric ??= type.IsGenericType)) continue;

                // Fill the Generic Type Definition
                if (0 == generic.HashCode) generic = contract.With(type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = container._scope.Get(in contract, in generic)))
                {
                    // Build from generic factory
                    return container.GenericRegistration(ref contract, manager, overrides);
                }
            }
            while (null != (container = container.Parent));

            // No registration found, resolve unregistered
            return (bool)isGeneric ? ResolveUnregisteredGeneric(ref contract, ref generic, overrides)
                  : type.IsArray   ? ResolveArray(ref contract, overrides)
                                   : ResolveUnregistered(ref contract, overrides);
        }

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
                    var value = Unsafe.As<LifetimeManager>(context.Registration).GetValue(_scope.Disposables);
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
                    ? ResolveArray(ref context.Contract, ref context)
                    : ResolveUnregistered(ref context.Contract, ref context);
        }
        
        #endregion


        #region Array

        private object? ResolveArray(ref Contract contract, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new PipelineContext(this, ref contract, ref request);

            context.Target = ResolveArray(ref contract, ref context);

            if (context.IsFaulted) throw new ResolutionFailedException(contract.Type, contract.Name, "");

            return context.Target;
        }

        private object? ResolveArray(ref Contract contract, ref PipelineContext context)
        {
            //return resolver(ref context);
            if (contract.Type.GetArrayRank() != 1)
            {
                return context.Error($"Invalid array {contract.Type}. Only arrays of rank 1 are supported");
            }

            ResolveDelegate<PipelineContext>? pipeline;
            if (null == (pipeline = _policies.Get<ResolveDelegate<PipelineContext>>(contract.Type)))
            { 
                var element = contract.Type.GetElementType();
                Debug.Assert(null != element);

                var target = ArrayTargetType(element!) ?? element;
                pipeline = !target.IsGenericType
                        ? ArrayMethod.MakeGenericMethod(element, target)
                                     .CreatePipeline()
                        : ArrayGeneric.MakeGenericMethod(element, target, target.GetGenericTypeDefinition())
                                      .CreatePipeline();
               
                pipeline = _policies.AddOrGet(context.Type, pipeline);
            }

             return pipeline(ref context);
        }

        #endregion


        #region Enumerable

        private ResolveDelegate<PipelineContext> ResolveEnumerable(ref Type type)
        {
            var element = type.GenericTypeArguments[0];
            Debug.Assert(null != element);

            var target = ArrayTargetType(element!) ?? element;
            var pipeline = !target.IsGenericType
                    ? EnumeratorMethod.MakeGenericMethod(element, target)
                                      .CreatePipeline()
                    : EnumeratorGeneric.MakeGenericMethod(element, target, target.GetGenericTypeDefinition())
                                       .CreatePipeline();

            return _policies.AddOrGet(type, pipeline);
        }

        #endregion


        #region ResolveAsync

        /// <inheritdoc />
        public ValueTask<object?> ResolveAsync(Type type, string? name, params ResolverOverride[] overrides)
        {
            var contract = new Contract(type, name);
            var container = this;

            do
            {
                RegistrationManager? manager;

                // Optimistic lookup
                if (null != (manager = container!._scope.Get(in contract)))
                {
                    object? value;

                    // Registration found, check for value
                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(_scope.Disposables)))
                        return new ValueTask<object?>(value);

                    // No value, do everything else asynchronously
                    return new ValueTask<object?>(Task.Factory.StartNew(container.ResolveContractAsync, new RequestInfoAsync(in contract, manager, overrides),
                        System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
                }
            }
            while (null != (container = container.Parent));

            // No registration found, do everything else asynchronously
            return new ValueTask<object?>(
                Task.Factory.StartNew(ResolveAsync, new RequestInfoAsync(in contract, overrides),
                    System.Threading.CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
        }

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
