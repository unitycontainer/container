using System;
using System.Collections.Generic;
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
            Set    = base.SetValue;
            Get    = base.GetValue;
            TryGet = base.TryGetValue;
        }

        #endregion


        #region SynchronizedLifetimeManager

        /// <inheritdoc/>
        public override object? GetValue(ICollection<IDisposable> lefetime)
        {
            return Get(lefetime);
        }

        /// <inheritdoc/>
        public override void SetValue(object? newValue, ICollection<IDisposable> lefetime)
        {
            Set(newValue, lefetime);
            Set = (o, c) => throw new InvalidOperationException("ContainerControlledLifetimeManager can only be set once");
            Get    = SynchronizedGetValue;
            TryGet = SynchronizedGetValue;
        }

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ICollection<IDisposable> lefetime) => Value;

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? newValue, ICollection<IDisposable> lefetime) => Value = newValue;

        #endregion


        #region IFactoryLifetimeManager

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ContainerControlledLifetimeManager
            {
                Scope = Scope
            };
        }

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


        #region Overrides

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerContainer"; 

        #endregion
    }
}
