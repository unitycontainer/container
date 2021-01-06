using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Resolve registration throwing exception in case of an error
        /// </summary>
        private object? ResolveRegistered(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new BuilderContext(this, ref contract, manager, ref request);

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
        private object? ResolveUnregistered(ref Contract contract, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var value = ResolveUnregistered(ref contract, ref request);

            if (request.IsFaulted)
            {
                // Throw if user exception
                request.ErrorInfo.Throw();

                // Throw ResolutionFailedException otherwise
                throw new ResolutionFailedException(in contract, request.ErrorInfo.Message);
            }

            return value;
        }
    }
}
