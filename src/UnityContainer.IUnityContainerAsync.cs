using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{

    public partial class UnityContainer : IUnityContainerAsync
    {
        #region Registration

        #region Type

        /// <inheritdoc />
        Task IUnityContainerAsync.RegisterType(IEnumerable<Type>? interfaces, Type type, string? name, ITypeLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return Task.Factory.StartNew((object status) =>
            {
                var types = status as Type[];

                Type? typeFrom = null;
                var registeredType = typeFrom ?? type;

                // Validate input
                //if (null == registeredType) throw new InvalidOperationException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must be not 'null'");

                try
                {
                    // Lifetime Manager
                    var manager = lifetimeManager as LifetimeManager ?? Context.TypeLifetimeManager.CreateLifetimePolicy();
                    if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
                    manager.InUse = true;

                    // Create registration and add to appropriate storage
                    var container = manager is SingletonLifetimeManager ? _root : this;
                    Debug.Assert(null != container);

                    // If Disposable add to container's lifetime
                    if (manager is IDisposable disposableManager)
                        container.LifetimeContainer.Add(disposableManager);

                    // Add or replace existing 
                    var registration = new ExplicitRegistration(container, name, type, manager, injectionMembers);
                    var previous = container.Register(registeredType, name, registration);

                    // Allow reference adjustment and disposal
                    if (null != previous && 0 == previous.Release()
                        && previous.LifetimeManager is IDisposable disposable)
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
                            member.AddPolicies<BuilderContext, ExplicitRegistration>(
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
                    if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString());
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
        Task IUnityContainerAsync.RegisterFactory(IEnumerable<Type> interfaces, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            throw new NotImplementedException();

            //// Validate input
            //// TODO: Move to diagnostic

            //if (null == interfaces) throw new ArgumentNullException(nameof(interfaces));
            //if (null == factory) throw new ArgumentNullException(nameof(factory));
            //if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
            //if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);

            //// Create registration and add to appropriate storage
            //var container = lifetimeManager is SingletonLifetimeManager ? _root : this;

            //// TODO: InjectionFactory
            //#pragma warning disable CS0618 
            //var injectionFactory = new InjectionFactory(factory);
            //#pragma warning restore CS0618

            //var injectionMembers = new InjectionMember[] { injectionFactory };
            //var registration = new ExplicitRegistration(_validators, (LifetimeManager)lifetimeManager, injectionMembers);

            //// Add Injection Members
            ////injectionFactory.AddPolicies<BuilderContext, ContainerRegistration>(
            ////    type, type, name, ref registration);

            //// Register interfaces
            //var replaced = container.AddOrReplaceRegistrations(interfaces, name, registration)
            //                        .ToArray();

            //// Release replaced registrations
            //if (0 != replaced.Length)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        foreach (ImplicitRegistration previous in replaced)
            //        {
            //            if (0 == previous.Release() && previous.LifetimeManager is IDisposable disposable)
            //            {
            //                // Dispose replaced lifetime manager
            //                container.LifetimeContainer.Remove(disposable);
            //                disposable.Dispose();
            //            }
            //        }
            //    });
            //}

            //return this;

        }

        #endregion


        #region Instance

        /// <inheritdoc />
        Task IUnityContainerAsync.RegisterInstance(IEnumerable<Type> interfaces, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            throw new NotImplementedException();

            //// Validate input
            //// TODO: Move to diagnostic

            //if (null == interfaces && null == instance) throw new ArgumentNullException(nameof(interfaces));

            //// Validate lifetime manager
            //if (null == lifetimeManager) lifetimeManager = new ContainerControlledLifetimeManager();
            //if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);
            //((LifetimeManager)lifetimeManager).SetValue(instance, LifetimeContainer);

            //// Create registration and add to appropriate storage
            //var mappedToType = instance?.GetType();
            //var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
            //var registration = new ExplicitRegistration(mappedToType, (LifetimeManager)lifetimeManager);

            //// If Disposable add to container's lifetime
            //if (lifetimeManager is IDisposable manager) container.LifetimeContainer.Add(manager);

            //// Register interfaces
            //var replaced = container.AddOrReplaceRegistrations(interfaces, name, registration)
            //                        .ToArray();

            //// Release replaced registrations
            //if (0 != replaced.Length)
            //{
            //    Task.Factory.StartNew(() => 
            //    {
            //        foreach (ImplicitRegistration previous in replaced)
            //        {
            //            if (0 == previous.Release() && previous.LifetimeManager is IDisposable disposable)
            //            {
            //                // Dispose replaced lifetime manager
            //                container.LifetimeContainer.Remove(disposable);
            //                disposable.Dispose();
            //            }
            //        }
            //    });
            //}

            //return this;
        }

        #endregion


        #endregion


        #region Resolution

        /// <inheritdoc />
        Task<object?> IUnityContainerAsync.Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            // Setup Context
            var registration = GetRegistration(type ?? throw new ArgumentNullException(nameof(type)), name);

            // Check if pipeline exists
            if (null == registration.Pipeline)
            {
                // Start a new Task to create and execute pipeline
                return Task.Factory.StartNew(() =>
                {
                    var context = new BuilderContext
                    {
                        Type = type,
                        Overrides = overrides,
                        Registration = registration,
                        ContainerContext = Context,
                    };

                    return context.Pipeline(ref context);
                });
            }

            // Existing pipeline
            var context = new BuilderContext
            {
                Async = true,

                Type = type,
                Overrides = overrides,
                Registration = registration,
                ContainerContext = Context,
            };

            // Execute the pipeline
            var task = context.Pipeline(ref context);

            // Make sure it is indeed a Task
            Debug.Assert(task is Task<object?>);

            // Return Task
            return (Task<object?>)task;
        }

        public Task<IEnumerable<object>> Resolve(Type type, Regex regex, params ResolverOverride[] overrides)
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
