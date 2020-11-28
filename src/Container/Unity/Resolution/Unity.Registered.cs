using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region  Throwing/Silent

        /// <summary>
        /// Resolve registration throwing exception in case of an error
        /// </summary>
        private object? ResolveRegistered(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new PipelineContext(this, ref contract, manager, ref request);

            ResolveRegistered(ref context);

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
        /// Silently resolve registration
        /// </summary>
        private object? ResolveRegistered(ref Contract contract, RegistrationManager manager)
        {
            var request = new RequestInfo(new ResolverOverride[0]);
            var context = new PipelineContext(this, ref contract, manager, ref request);

            ResolveRegistered(ref context);

            return request.IsFaulted ? null : context.Target;
        }

        #endregion


        #region Implementation

        /// <summary>
        /// Resolve existing registration
        /// </summary>
        private object? ResolveRegistered(ref PipelineContext context)
        {
            var manager = context.Registration!;

            // Double lock check and create pipeline
            if (manager.Pipeline is null) lock (manager) if (manager.Pipeline is null)
                        manager.Pipeline = BuildPipelineRegistered(ref context);

            // Resolve
            try
            {
                using var scope = context.CreateScope(this);
                
                context.Target = manager.Pipeline!(ref context);
            }
            catch (Exception ex)
            {
                context.Capture(ex);
            }

            // Handle errors, if any
            if (context.IsFaulted)
            {
                if (manager is SynchronizedLifetimeManager synchronized)
                    synchronized.Recover();

                return RegistrationManager.NoValue;
            }

            // Save resolved value
            manager.SetValue(context.Target, _scope);

            return context.Target;
        }

        #endregion
    }
}
