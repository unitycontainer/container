using System;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Registered Contract


        private object? ResolveContract(in ResolutionContext context)
        {
            return new object();

            //// Check if pipeline has been created already
            //if (null == context.Manager.ResolveDelegate)
            //{
            //    // Prevent creating pipeline multiple times
            //    lock (context.Manager)
            //    {
            //        // Make sure it is still not created while waited for the lock
            //        if (null == context.Manager.ResolveDelegate)
            //        {
            //            var lifetime = (LifetimeManager)context.Manager;

            //            context.Manager.ResolveDelegate = context.Manager.Category switch
            //            {
            //                RegistrationCategory.Instance => _policies.InstancePipeline,
            //                RegistrationCategory.Factory => _policies.FactoryPipeline,

            //                RegistrationCategory.Clone when ResolutionStyle.OnceInLifetime == lifetime.Style => _policies.TypePipeline,
            //                //RegistrationCategory.Clone when ResolutionStyle.OnceInWhile == lifetime.Style => _policies.BalancedPipelineFactory(in contract, context.Manager),
            //                //RegistrationCategory.Clone when ResolutionStyle.EveryTime == lifetime.Style  => _policies.OptimizedPipelineFactory(in contract, context.Manager),

            //                //RegistrationCategory.Type when ResolutionStyle.OnceInLifetime == lifetime.Style => _policies.TypePipeline,
            //                //RegistrationCategory.Type when ResolutionStyle.OnceInWhile == lifetime.Style => _policies.BalancedPipelineFactory(in contract, context.Manager),
            //                //RegistrationCategory.Type when ResolutionStyle.EveryTime == lifetime.Style  => _policies.OptimizedPipelineFactory(in contract, context.Manager),

            //                _ => throw new InvalidOperationException($"Registration {context.Type}/{context.Name} has unsupported category {context.Manager.Category}")
            //            };
            //        }
            //    }
            //}

            //// Resolve in current context
            //return ((ResolveDelegate<ResolutionContext>)context.Manager.ResolveDelegate)(ref context);
        }

        #endregion


        #region Unregistered

        /// <summary>
        /// Resolve unregistered <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// Although <see cref="Contract"/> is used as an input, but only <see cref="Type"/> is
        /// used to identify correct entry.
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to use for resolution</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested object</returns>
        private object? ResolveUnregistered(in Contract contract, ResolverOverride[] overrides)
        {
            // Check if resolver already exist
            var resolver = _policies[contract.Type];

            // Nothing found, requires build
            if (null == resolver)
            {
                // Build new and try to save it
                resolver = _policies.UnregisteredPipelineFactory(in contract);
                resolver = _policies.GetOrAdd(contract.Type, resolver);
            }

            var context = new ResolveContext(this, in contract, overrides);
            return resolver(ref context);
        }

        /// <summary>
        /// Resolve unregistered generic <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// Although <see cref="Contract"/> is used as an input, but only <see cref="Type"/> is
        /// used to identify correct entry.
        /// This method will first look for a type factory, before invoking default resolver factory
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to use for resolution</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested object</returns>
        private object? ResolveUnregisteredGeneric(in Contract contract, in Contract generic, ResolverOverride[] overrides)
        {
            var context = new ResolveContext(this, in contract, overrides);

            // Check if resolver already exist
            var resolver = _policies[contract.Type];
            if (null != resolver) return resolver(ref context);

            var factory = _policies.Get<ResolveDelegateFactory>(generic.Type);
            if (null != factory)
            {
                // Build from factory and try to store it
                resolver = factory(in contract);
                resolver = _policies.GetOrAdd(contract.Type, resolver);
                return resolver(ref context);
            }

            // Build new and try to save it
            resolver = _policies.UnregisteredPipelineFactory(in contract);
            resolver = _policies.GetOrAdd(contract.Type, resolver);

            // Resolve
            return resolver(ref context);
        }

        #endregion


        #region Array

        /// <summary>
        /// Resolve array
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> the array factory will be stored at</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested array</returns>
        private object? ResolveArray(in Contract contract, ResolverOverride[] overrides)
        {
            var context = new ResolveContext(this, in contract, overrides);
            var resolver = _policies[contract.Type];

            // Nothing found, requires build
            if (null == resolver)
            {
                resolver = (ref ResolveContext c) => c.Existing;
                _policies[contract.Type] = resolver;
            }

            return resolver(ref context);
        }

        #endregion


        #region Resolve Async

        /// <summary>
        /// Builds and resolves registered contract
        /// </summary>
        /// <param name="state"><see cref="ResolveContractAsyncState"/> objects holding 
        /// resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveContractAsync(object? state)
        {
            ResolveContractAsyncState context = (ResolveContractAsyncState)state!;



            return Task.FromResult<object?>(context.Manager);
        }

        /// <summary>
        /// Builds and resolves unregistered <see cref="Type"/>
        /// </summary>
        /// <param name="state"><see cref="ResolveAsyncState"/> objects holding resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveAsync(object? state)
        {
            ResolveAsyncState context = (ResolveAsyncState)state!;



            return Task.FromResult<object?>(context.Contract.Type);
        }

        #region State Objects

        /// <summary>
        /// Internal state passed to <see cref="ResolveContractAsync"/>
        /// </summary>
        private class ResolveContractAsyncState
        {
            public readonly Contract Contract;
            public readonly RegistrationManager Manager;
            public readonly ResolverOverride[] Overrides;

            public ResolveContractAsyncState(in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
            {
                Contract = contract;
                Manager = manager;
                Overrides = overrides;
            }
        }

        /// <summary>
        /// Internal state passed to <see cref="ResolveAsync"/>
        /// </summary>
        private class ResolveAsyncState
        {
            public readonly Contract Contract;
            public readonly ResolverOverride[] Overrides;

            public ResolveAsyncState(in Contract contract, ResolverOverride[] overrides)
            {
                Contract = contract;
                Overrides = overrides;
            }
        }


        #endregion

        #endregion
    }
}
