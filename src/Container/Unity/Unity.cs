using System;
using Unity.BuiltIn;
using Unity.Container;
using Unity.Pipeline;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        internal Scope   _scope;
        internal readonly Defaults _policies;

        #endregion


        #region Constructors

        /// <summary>
        /// Default <see cref="UnityContainer"/> constructor
        /// </summary>
        public UnityContainer(string? name = "root")
        {
            Root = this;
            Name = name;

            _policies = new Defaults();
            _context  = new PrivateExtensionContext(this);

            // Setup Processors
            ConstructorProcessor.SetupProcessor(_context);
                  FieldProcessor.SetupProcessor(_context);
               PropertyProcessor.SetupProcessor(_context);
                 MethodProcessor.SetupProcessor(_context);

            // Registration Scope
            _scope = new ContainerScope();
            _scope.Add(new ContainerLifetimeManager(this), 
                typeof(IUnityContainer), 
                typeof(IUnityContainerAsync), 
                typeof(IServiceProvider));
        }

        /// <summary>
        /// Child container constructor
        /// </summary>
        /// <param name="parent">Parent <see cref="UnityContainer"/></param>
        /// <param name="name">Name of this container</param>
        protected UnityContainer(UnityContainer parent, string? name = null)
        {
            Name   = name;
            Root   = parent.Root;
            Parent = parent;

            _policies = parent.Root._policies;
            
            // Registration Scope
            _scope = parent._scope.CreateChildScope();
            _scope.Add(new ContainerLifetimeManager(this),
                typeof(IUnityContainer), typeof(IUnityContainerAsync), typeof(IServiceProvider));
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            // Child container dispose
            if (null != Parent) Parent.Registering -= OnParentRegistering; 

            _scope.Dispose();
        }

        #endregion
    }
}
