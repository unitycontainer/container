using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Registered

        private object? ResolveThrowingOnError(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new PipelineContext(ref contract, manager, ref request, this);

            try
            {
                // Double lock check and create pipeline
                if (null == manager.Pipeline) lock (manager) if (null == manager.Pipeline)
                    manager.Pipeline = BuildPipeline(ref context);

                // Resolve
                context.Target = manager.Pipeline!(ref context);
                context.LifetimeManager?.SetValue(context.Target, _scope);
            }
            catch when (manager is SynchronizedLifetimeManager synchronized)
            {
                synchronized.Recover();
                throw;
            }

            if (request.IsFaulted) 
                throw new ResolutionFailedException(contract.Type, contract.Name, request.Error.Message!);

            return context.Target;
        }

        private object? ResolveSilent(ref Contract contract, RegistrationManager manager)
        {
#if NET45
            var request = new RequestInfo(new ResolverOverride[0]);
#else
            var request = new RequestInfo(Array.Empty<ResolverOverride>());
#endif
            var context = new PipelineContext(ref contract, manager, ref request, this);

            try
            {
                // Double lock check and create pipeline
                if (null == manager.Pipeline) lock (manager) if (null == manager.Pipeline)
                            manager.Pipeline = BuildPipeline(ref context);

                // Resolve
                context.Target = manager.Pipeline!(ref context);
                context.LifetimeManager?.SetValue(context.Target, _scope);
            }
            catch when (manager is SynchronizedLifetimeManager synchronized)
            {
                synchronized.Recover();
            }

            return request.IsFaulted ? null : context.Target;
        }

        #endregion


        #region Unregistered

        private object? ResolveThrowingOnError(ref Contract contract, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new PipelineContext(ref contract, ref request, this);

            try
            {
                Resolve(ref context);
            }
            catch(Exception ex)
            {
                if (context.Registration is SynchronizedLifetimeManager manager)
                    manager.Recover();
                
                context.Exception(ex);
            }

            if (request.IsFaulted) throw new ResolutionFailedException(ref context);

            return context.Target;
        }

        private object? ResolveSilent(ref Contract contract)
        {
#if NET45
            var request = new RequestInfo(new ResolverOverride[0]);
#else
            var request = new RequestInfo(Array.Empty<ResolverOverride>());
#endif
            var context = new PipelineContext(ref contract, ref request, this);

            try
            {
                Resolve(ref context);
            }
            catch
            {
                if (context.Registration is SynchronizedLifetimeManager manager)
                    manager.Recover();

                return null;
            }

            return request.IsFaulted ? null : context.Target;
        }

        #endregion
    }
}
