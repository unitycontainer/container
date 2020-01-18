using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{

    public partial class UnityContainer : IUnityContainerAsync
    {
        #region Registration

        #region Type

        /// <inheritdoc />
        Task IUnityContainerAsync.RegisterType(IEnumerable<Type>? interfaces, Type type, string? name, ITypeLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return Task.Factory.StartNew((object? status) =>
            {
                var types = status as Type[];

                Type? typeFrom = null;
                var registeredType = typeFrom ?? type;

                // Validate input
                //if (null == registeredType) throw new InvalidOperationException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must be not 'null'");

                try
                {
                    // Lifetime Manager
                    var manager = lifetimeManager as LifetimeManager ?? Context.TypeLifetimeManager.Clone();

                    // Determine scope
                    var container = manager is SingletonLifetimeManager ? _root : this;

                    // If Disposable add to container's lifetime
                    if (manager is IDisposable disposableManager)
                        container.LifetimeContainer.Add(disposableManager);

                    // Add or replace existing 
                    var registration = new ExplicitRegistration(container, name, type, manager, injectionMembers);
                    var previous = container.RegisterType(registeredType, name, registration);

                    // Allow reference adjustment and disposal
                    if (null != previous && previous is IDisposable disposable)
                    {
                        // Dispose replaced lifetime manager
                        container.LifetimeContainer.Remove(disposable);
                        disposable.Dispose();
                    }

                    // Add Injection Members
                    if (null != injectionMembers && injectionMembers.Length > 0)
                    {
                        foreach (var member in injectionMembers)
                        {
                            member.AddPolicies<PipelineContext, ExplicitRegistration>(
                                registeredType, type, name, ref registration);
                        }
                    }

                    // Check what strategies to run
                    registration.Processors = Context.TypePipelineCache;

                    // Raise event
                    //container.Registering?.Invoke(this, new RegisterEventArgs(registeredType,
                    //                                                          typeTo,
                    //                                                          name,
                    //                                                          manager));
                }
                catch (Exception ex)
                {
                    var builder = new StringBuilder();

                    builder.AppendLine(ex.Message);
                    builder.AppendLine();

                    var parts = new List<string>();
                    var generics = null == typeFrom ? type?.Name : $"{typeFrom?.Name},{type?.Name}";
                    if (null != name) parts.Add($" '{name}'");
                    if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString()!);
                    if (null != injectionMembers && 0 != injectionMembers.Length)
                        parts.Add(string.Join(" ,", injectionMembers.Select(m => m.ToString())));

                    builder.AppendLine($"  Error in:  RegisterType<{generics}>({string.Join(", ", parts)})");
                    throw new InvalidOperationException(builder.ToString(), ex);
                }
            }, ValidateTypes(interfaces, type));
        }

        #endregion


        #region Factory

        /// <inheritdoc />
        Task IUnityContainerAsync.RegisterFactory(IEnumerable<Type>? interfaces, string? name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager? lifetimeManager)
        {
            throw new NotImplementedException();
            //// Validate input
            //if (null == interfaces) throw new ArgumentNullException(nameof(interfaces));
            //if (null == factory) throw new ArgumentNullException(nameof(factory));

            //return Task.Factory.StartNew(() =>
            //{
            //    // TODO: implementation required
            //    var type = interfaces.First();

            //    // Lifetime Manager
            //    var manager = lifetimeManager as LifetimeManager ?? Context.FactoryLifetimeManager.CreateLifetimePolicy();
            //    if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
            //    manager.InUse = true;

            //    // Target Container
            //    var container = manager is SingletonLifetimeManager ? _root : this;
            //    Debug.Assert(null != container);

            //    // If Disposable add to container's lifetime
            //    if (manager is IDisposable managerDisposable)
            //        container.LifetimeContainer.Add(managerDisposable);

            //    // Create registration
            //    var registration = new ExplicitRegistration(container, name, type, manager);

            //    // Factory resolver
            //    var resolver = lifetimeManager is PerResolveLifetimeManager
            //        ? (ResolveDelegate<BuilderContext>)((ref BuilderContext c) =>
            //        {
            //            c.Existing = factory(c.Container, c.Type, c.Name);
            //            c.Set(typeof(LifetimeManager), new InternalPerResolveLifetimeManager(c.Existing));
            //            return c.Existing;
            //        })
            //        : ((ref BuilderContext c) => factory(c.Container, c.Type, c.Name));
            //    registration.Set(typeof(ResolveDelegate<BuilderContext>), resolver);

            //    // Build Pipeline
            //    PipelineBuilder builder = new PipelineBuilder(registration, container, Context.FactoryPipelineCache);
            //    registration.Pipeline = builder.Pipeline();

            //    // Register
            //    var previous = container.Register(type, name, registration);

            //    // Allow reference adjustment and disposal
            //    if (null != previous && 0 == previous.Release()
            //        && previous.LifetimeManager is IDisposable disposable)
            //    {
            //        // Dispose replaced lifetime manager
            //        container.LifetimeContainer.Remove(disposable);
            //        disposable.Dispose();
            //    }

            //    // TODO: Raise event
            //    // container.Registering?.Invoke(this, new RegisterEventArgs(type, type, name, manager));
            //});
        }

        #endregion


        #region Instance

        /// <inheritdoc />
        Task IUnityContainerAsync.RegisterInstance(IEnumerable<Type>? interfaces, string? name, object? instance, IInstanceLifetimeManager? lifetimeManager)
        {
            // Validate input
            throw new NotImplementedException();
        }

        #endregion


        #endregion


        #region Resolution

        /// <inheritdoc />
        [SecuritySafeCritical]
        ValueTask<object?> IUnityContainerAsync.ResolveAsync(Type type, string? name, params ResolverOverride[] overrides)
        {
            var key = new HashKey(type ?? throw new ArgumentNullException(nameof(type)), name);
            var pipeline = TryGetPipeline(ref key);

            // Check if already got value
            if (pipeline?.Target is LifetimeManager manager)
            {
                // Make unblocking check for result
                var value = manager.TryGet(LifetimeContainer);
                if (LifetimeManager.NoValue != value) return new ValueTask<object?>(value);
            }

            return new ValueTask<object?>(Task.Factory.StartNew<object?>(delegate 
            {
                if (null == pipeline) pipeline = GetPipeline(ref key);

                // Check if already created and acquire a lock if not
                var manager = pipeline.Target as LifetimeManager;
                if (null != manager) // TODO: this is not required
                {
                    // Make blocking check for result
                    var value = manager.Get(LifetimeContainer);
                    if (LifetimeManager.NoValue != value) return value;
                }

                // Setup Context
                var synchronized = manager as SynchronizedLifetimeManager;
                var context = new PipelineContext
                {
                    List = new PolicyList(),
                    Type = type,
                    Overrides = overrides,
                    ContainerContext = null == manager?.Scope
                                     ? Context
                                     : (ContainerContext)manager.Scope,
                };

                // Execute pipeline
                try
                {
                    var value = pipeline(ref context);
                    manager?.Set(value, LifetimeContainer);
                    return value;
                }
                catch (Exception ex)
                {
                    synchronized?.Recover();

                    if (ex is InvalidRegistrationException ||
                        ex is CircularDependencyException  ||
                        ex is ObjectDisposedException)
                    {
                        var message = CreateErrorMessage(ex);
                        throw new ResolutionFailedException(context.Type, context.Name, message, ex);
                    }
                    else throw;
                }
            }));
        }

        public ValueTask<IEnumerable<object?>> Resolve(Type type, Regex regex, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Child container management

        /// <inheritdoc />
        IUnityContainerAsync IUnityContainerAsync.CreateChildContainer() => CreateChildContainer();

        /// <inheritdoc />
        IUnityContainerAsync? IUnityContainerAsync.Parent => _parent;

        #endregion
    }
}
