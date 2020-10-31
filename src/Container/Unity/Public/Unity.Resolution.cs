using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <inheritdoc />
        [SkipLocalsInit]
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            Contract contract = new Contract(type, name);
            RequestInfo request;
            PipelineContext context;
            RegistrationManager? manager;

            // Look for registration
            if (null != (manager = _scope.Get(in contract)))
            {
                //Registration found, check value
                var value = manager.TryGetValue(_scope);
                if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                // Resolve registration
                request = new RequestInfo(overrides);
                context = new PipelineContext(ref contract, manager, ref request, this);
                
                ResolveRegistration(ref context);

                if (request.IsFaulted) throw new ResolutionFailedException(ref context);
                
                return context.Target;
            }

            // Resolve 
            request = new RequestInfo(overrides);
            context = new PipelineContext(ref contract, ref request, this);
            context.Target = Resolve(ref context);

            if (request.IsFaulted) throw new ResolutionFailedException(ref context);

            return context.Target;
        }
        

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
                    if (RegistrationManager.NoValue != (value = manager.TryGetValue(_scope)))
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
    }
}
