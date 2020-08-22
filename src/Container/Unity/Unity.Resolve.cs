using System;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Registered Contract

        /// <summary>
        /// Resolve registered contract
        /// </summary>
        /// <remarks>
        /// The registration could be for a <see cref="Contract"/> itself or for generic factory.
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to resolve</param>
        /// <param name="manager"><see cref="RegistrationManager"/> holding information about the registration</param>
        /// <param name="overrides">An array of <see cref="ResolverOverride"/> objects to use during resolution</param>
        /// <exception cref="ResolutionFailedException">Thrown if for some reason object could not be resolved</exception>
        /// <returns>Returns resolved object</returns>
        private object? ResolveContract(in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            // Check if pipeline has been created already
            if (null == manager.ResolveDelegate)
            {
                // Prevent creating pipeline multiple times
                lock (manager)
                {
                    // Make sure it is still not created while waited for the lock
                    if (null == manager.ResolveDelegate)
                        manager.ResolveDelegate = _policies.DelegateFactory(in contract, manager);
                }
            }

            // Resolve in current context
            ResolveContext context = new ResolveContext(this, in contract, manager, overrides);
            return ((ResolveDelegate<ResolveContext>)manager.ResolveDelegate)(ref context);
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
                resolver = _policies.DelegateFactory(in contract);
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
            resolver = _policies.DelegateFactory(in contract);
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
