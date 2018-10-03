using System;
using Unity.Lifetime;

namespace Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that is unique for all the children containers.
    /// When the <see cref="SingletonLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </summary>
    public class SingletonLifetimeManager : SynchronizedLifetimeManager,
                                            ISingletonLifetimePolicy
    {
        #region Fields

        protected object Value;

        #endregion

        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return Value;
        }

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container"></param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.SetValue"/>
        /// before releasing its lock.</remarks>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            Value = newValue;
        }


        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        /// <param name="container">Instance of container</param>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            Dispose();
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new SingletonLifetimeManager();
        }

        #region IDisposable

        /// <summary>		
        /// Standard Dispose pattern implementation.		
        /// </summary>		
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>		
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
    }
}
