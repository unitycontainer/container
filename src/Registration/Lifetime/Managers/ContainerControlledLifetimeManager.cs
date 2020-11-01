using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// <para>
    /// Unity returns the same instance each time the Resolve(...) method is called or when the
    /// dependency mechanism injects the instance into other classes.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per Container lifetime allows a registration of an existing or resolved object as 
    /// a scoped singleton in the container it was created or registered. In other words 
    /// this instance is unique within the container it war registered with. Child or parent 
    /// containers could have their own instances registered for the same contract.
    /// </para>
    /// <para>
    /// When the <see cref="ContainerControlledLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </para>
    /// </remarks>
    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager, 
                                                      IInstanceLifetimeManager, 
                                                      IFactoryLifetimeManager,
                                                      ITypeLifetimeManager
    {
        #region Fields

        /// <summary>
        /// An instance of the object this manager is associated with.
        /// </summary>
        /// <value>This field holds a strong reference to the associated object.</value>
        protected object? Value = NoValue;

        #endregion


        #region Constructor

        public ContainerControlledLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? TryGetValue(ICollection<IDisposable> scope)
            => Value;

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ICollection<IDisposable> scope) 
            => Value;

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? newValue, ICollection<IDisposable> scope) 
            => Value = newValue;

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.OnceInLifetime;

        /// <inheritdoc/>
        public override ImportSource Source => ImportSource.NonLocal;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new ContainerControlledLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerContainer"; 

        #endregion


        #region IDisposable

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (NoValue == Value) return;
                if (Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                Value = NoValue;
            }
            finally 
            {
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}
