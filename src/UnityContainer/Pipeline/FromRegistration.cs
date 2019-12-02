using System;
using System.Diagnostics;
using System.Threading;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private FromRegistration PipelineFromRegistration { get; }


        #region Optimized

        private ResolveDelegate<PipelineContext> PipelineFromRegistrationOptimized(Type? type, ExplicitRegistration registration, int position)
        {
            var manager = registration.LifetimeManager;
            ResolveDelegate<PipelineContext>? pipeline = null;

            Debug.Assert(null != type);
            Debug.Assert(null != manager);

            lock (_syncRegistry)
            {
                Debug.Assert(null != _registry);

                ref var entry = ref _registry!.Entries[position];

                if (ReferenceEquals(entry.Registration, registration) && null == entry.Pipeline)
                {
                    entry.Pipeline = manager!.Pipeline;
                    manager.PipelineDelegate = (ResolveDelegate<PipelineContext>)SpinWait;
                }
            }

            lock (manager!)
            {
                if ((Delegate)(ResolveDelegate<PipelineContext>)SpinWait == manager.PipelineDelegate)
                {
                    PipelineBuilder builder = new PipelineBuilder(type!, registration);
                    manager.PipelineDelegate = builder.Pipeline();

                    Debug.Assert(null != manager.PipelineDelegate);
                    pipeline = (ResolveDelegate<PipelineContext>)manager.PipelineDelegate!;
                }
            }

            return manager.Pipeline;


            object? SpinWait(ref PipelineContext context)
            {
                while (null == pipeline)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                return pipeline(ref context);
            }
        }

        #endregion


        #region Activated

        private ResolveDelegate<PipelineContext> PipelineFromRegistrationActivated(Type? type, ExplicitRegistration registration, int position)
        {
            var manager = registration.LifetimeManager;
            ResolveDelegate<PipelineContext>? pipeline = null;

            Debug.Assert(null != type);
            Debug.Assert(null != manager);

            lock (_syncRegistry)
            {
                Debug.Assert(null != _registry);

                ref var entry = ref _registry!.Entries[position];

                if (ReferenceEquals(entry.Registration, registration) && null == entry.Pipeline)
                {
                    entry.Pipeline = manager!.Pipeline;
                    manager.PipelineDelegate = (ResolveDelegate<PipelineContext>)SpinWait;
                }
            }

            lock (manager!)
            {
                if ((Delegate)(ResolveDelegate<PipelineContext>)SpinWait == manager.PipelineDelegate)
                {
                    PipelineBuilder builder = new PipelineBuilder(type!, registration);
                    manager.PipelineDelegate = builder.Pipeline();

                    Debug.Assert(null != manager.PipelineDelegate);
                    pipeline = (ResolveDelegate<PipelineContext>)manager.PipelineDelegate!;
                }
            }

            return manager.Pipeline;


            object? SpinWait(ref PipelineContext context)
            {
                while (null == pipeline)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                return pipeline(ref context);
            }
        }

        #endregion


        #region Compiled

        private ResolveDelegate<PipelineContext> PipelineFromRegistrationCompiled(Type? type, ExplicitRegistration registration, int position)
        {
            var manager = registration.LifetimeManager;
            ResolveDelegate<PipelineContext>? pipeline = null;

            Debug.Assert(null != type);
            Debug.Assert(null != manager);

            lock (_syncRegistry)
            {
                Debug.Assert(null != _registry);

                ref var entry = ref _registry!.Entries[position];

                if (ReferenceEquals(entry.Registration, registration) && null == entry.Pipeline)
                {
                    entry.Pipeline = manager!.Pipeline;
                    manager.PipelineDelegate = (ResolveDelegate<PipelineContext>)SpinWait;
                }
            }

            lock (manager!)
            {
                if ((Delegate)(ResolveDelegate<PipelineContext>)SpinWait == manager.PipelineDelegate)
                {
                    PipelineBuilder builder = new PipelineBuilder(type!, registration);
                    manager.PipelineDelegate = builder.Compile();

                    Debug.Assert(null != manager.PipelineDelegate);
                    pipeline = (ResolveDelegate<PipelineContext>)manager.PipelineDelegate!;
                }
            }

            return manager.Pipeline;


            object? SpinWait(ref PipelineContext context)
            {
                while (null == pipeline)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                return pipeline(ref context);
            }
        }

        #endregion
    }
}
