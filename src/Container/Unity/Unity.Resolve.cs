using System;
using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Resolve Contract

        private object? ResolveContract(in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            object strategy = manager.Category switch
            {
                RegistrationCategory.Internal => throw new NotImplementedException(),
                RegistrationCategory.Type => throw new NotImplementedException(),
                RegistrationCategory.Instance => throw new NotImplementedException(),
                RegistrationCategory.Factory => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            return null;
        }

        private object? ResolveFromGeneric(in Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            return null;
        }

        #endregion


        #region Resolve Unregistered

        /// <summary>
        /// Resolve unregistered <see cref="Type"/>
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> to use for resolution</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested object</returns>
        private object? ResolveUnregistered(in Contract contract, ResolverOverride[] overrides)
        {
            var context = new ContainerContext(this, in contract, overrides);

            // Check if resolver already exist
            var resolver = _policies[contract.Type];

            if (null == resolver)
            {
                resolver = _policies.UnregisteredFactory(in context);
                _policies[contract.Type] = resolver;
            }

            return resolver(ref context.ResolveContext);
        }

        private object? ResolveUnregisteredGeneric(in Contract contract, in Contract generic, ResolverOverride[] overrides)
        {
            return null;
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
            return null;
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
