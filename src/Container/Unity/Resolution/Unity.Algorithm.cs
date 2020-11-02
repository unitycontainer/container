using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private object? ResolveThrowingOnError(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            object? value;
            var request = new RequestInfo(overrides);

            try
            {
                value = ResolveRegistration(ref contract, manager, ref request);
            }
            catch when (manager is SynchronizedLifetimeManager synchronized)
            {
                synchronized.Recover();
                throw;
            }

            if (request.IsFaulted) throw new ResolutionFailedException(contract.Type, contract.Name, request.Error.Message!);

            return value;
        }

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
                ResolveRegistration(ref context);
            }
            catch
            {
                // TODO: optimize
                if (context.Registration is SynchronizedLifetimeManager synchronized)
                    synchronized.Recover();

                return null;
            }

            return request.IsFaulted ? null : context.Target;
        }
    }
}
