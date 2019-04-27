using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        IUnityContainer IUnityContainerAsync.RegisterType(IEnumerable<Type> interfaces, Type type, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Factory

        /// <inheritdoc />
        IUnityContainer IUnityContainerAsync.RegisterFactory(IEnumerable<Type> interfaces, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            // Validate input
            // TODO: Move to diagnostic

            if (null == interfaces) throw new ArgumentNullException(nameof(interfaces));
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
            if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);

            // Create registration and add to appropriate storage
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;

            // TODO: InjectionFactory
            #pragma warning disable CS0618 
            var injectionFactory = new InjectionFactory(factory);
            #pragma warning restore CS0618

            var injectionMembers = new InjectionMember[] { injectionFactory };
            var registration = new ExplicitRegistration(_validators, (LifetimeManager)lifetimeManager, injectionMembers);

            // Add Injection Members
            //injectionFactory.AddPolicies<BuilderContext, ContainerRegistration>(
            //    type, type, name, ref registration);

            // Register interfaces
            var replaced = container.AddOrReplaceRegistrations(interfaces, name, registration)
                                    .ToArray();

            // Release replaced registrations
            if (0 != replaced.Length)
            {
                Task.Factory.StartNew(() =>
                {
                    foreach (ImplicitRegistration previous in replaced)
                    {
                        if (0 == previous.Release() && previous.LifetimeManager is IDisposable disposable)
                        {
                            // Dispose replaced lifetime manager
                            container.LifetimeContainer.Remove(disposable);
                            disposable.Dispose();
                        }
                    }
                });
            }

            return this;

        }

        #endregion


        #region Instance

        /// <inheritdoc />
        IUnityContainer IUnityContainerAsync.RegisterInstance(IEnumerable<Type> interfaces, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            // Validate input
            // TODO: Move to diagnostic

            if (null == interfaces && null == instance) throw new ArgumentNullException(nameof(interfaces));

            // Validate lifetime manager
            if (null == lifetimeManager) lifetimeManager = new ContainerControlledLifetimeManager();
            if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);
            ((LifetimeManager)lifetimeManager).SetValue(instance, LifetimeContainer);

            // Create registration and add to appropriate storage
            var mappedToType = instance?.GetType();
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
            var registration = new ExplicitRegistration(mappedToType, (LifetimeManager)lifetimeManager);

            // If Disposable add to container's lifetime
            if (lifetimeManager is IDisposable manager) container.LifetimeContainer.Add(manager);

            // Register interfaces
            var replaced = container.AddOrReplaceRegistrations(interfaces, name, registration)
                                    .ToArray();
            
            // Release replaced registrations
            if (0 != replaced.Length)
            {
                Task.Factory.StartNew(() => 
                {
                    foreach (ImplicitRegistration previous in replaced)
                    {
                        if (0 == previous.Release() && previous.LifetimeManager is IDisposable disposable)
                        {
                            // Dispose replaced lifetime manager
                            container.LifetimeContainer.Remove(disposable);
                            disposable.Dispose();
                        }
                    }
                });
            }

            return this;
        }

        #endregion


        #endregion


        #region Hierarchy

        /// <inheritdoc />
        IUnityContainer IUnityContainerAsync.Parent => throw new NotImplementedException();

        /// <inheritdoc />
        IUnityContainer IUnityContainerAsync.CreateChildContainer()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Resolution

        /// <inheritdoc />
        Task<object> IUnityContainerAsync.Resolve(Type type, string name, params ResolverOverride[] overrides)
        {
            return null;// _getPipeline(type, name).Invoke(this, overrides);
        }

        public Task<IEnumerable<object>> Resolve(Type type, Regex regex, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
