using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Throwing

        /// <summary>
        /// Resolve registration throwing exception in case of an error
        /// </summary>
        private object? RegisteredThrowing(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var request = BuilderContext.NewRequest(overrides);
            var context = request.Context(this, ref contract, manager);

            Policies.ResolveRegistered(ref context);

            if (request.IsFaulted)
            {
                // Throw if user exception
                request.ErrorInfo.Throw();

                // Throw ResolutionFailedException otherwise
                throw new ResolutionFailedException(in contract, request.ErrorInfo.Message);
            }

            return context.Target;
        }


        /// <summary>
        /// Resolve unknown type throwing exception in case of an error
        /// </summary>
        private object? UnregisteredThrowing(ref Contract contract, ResolverOverride[] overrides)
        {
            var request = BuilderContext.NewRequest(overrides);
            var context = request.Context(this, ref contract);

            ResolveUnregistered(ref context);

            if (request.IsFaulted)
            {
                // Throw if user exception
                request.ErrorInfo.Throw();

                // Throw ResolutionFailedException otherwise
                throw new ResolutionFailedException(in contract, request.ErrorInfo.Message);
            }

            return context.Target;
        }

        #endregion


        #region Silent

        /// <summary>
        /// Silently resolve registration
        /// </summary>
        private object? RegisteredSilent(ref Contract contract, RegistrationManager manager)
        {
            var request = BuilderContext.NewRequest();
            var context = request.Context(this, ref contract, manager);

            Policies.ResolveRegistered(ref context);

            return request.IsFaulted ? null : context.Target;
        }


        /// <summary>
        /// Silently resolve unknown type
        /// </summary>
        private object? UnregisteredSilent(ref Contract contract)
        {
            var request = BuilderContext.NewRequest();
            var context = request.Context(this, ref contract);

            ResolveUnregistered(ref context);
            
            return request.IsFaulted ? null : context.Target;
        }

        #endregion
    }
}
