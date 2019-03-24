using System;

namespace Unity.Lifetime
{
    /// <summary>
    /// Singleton lifetime creates globally unique singleton. Any Unity container tree 
    /// (parent and all the children) is guaranteed to have only one global singleton 
    /// for the registered type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Registering a type with singleton lifetime always places the registration 
    /// at the root of the container tree and makes it globally available for all 
    /// the children of that container. It does not matter if registration takes 
    /// places at the root of child container the destination is always the root node.
    /// </para>
    /// <para>
    /// Repeating the registration on any of the child nodes with singleton lifetime 
    /// will always override the root registration.
    /// </para>
    /// <para>
    /// When the <see cref="SingletonLifetimeManager"/> is disposed, the instance it holds 
    /// is disposed with it.</para>
    /// </remarks>
    public class SingletonLifetimeManager : SynchronizedLifetimeManager,
                                            IInstanceLifetimeManager,
                                            IFactoryLifetimeManager,
                                            ITypeLifetimeManager
    {
        #region Fields

        /// <summary>
        /// An instance of the singleton object this manager is associated with.
        /// </summary>
        /// <value>This field holds a strong reference to the singleton object.</value>
        protected object Value;

        #endregion

        /// <inheritdoc/>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return Value;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            Value = newValue;
        }

        /// <inheritdoc/>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            Dispose();
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new SingletonLifetimeManager();
        }

        #region IDisposable

        /// <inheritdoc/>		
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Value == null) return;
            if (Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
            Value = null;
        }

        #endregion


        #region Overrides

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:Singleton";

        #endregion
    }
}
