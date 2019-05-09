using System;

namespace Unity.Lifetime
{
    /// <summary>
    /// This lifetime manager keeps a weak reference to an associated object.
    /// </summary>
    /// <remarks>
    /// <para>The manager gets and holds a weak reference to the object. As long as the object still 
    /// exists and has not been garbage collected the container will return the object when requested.</para>
    /// <para>If the object went out of scope and has been garbage collected the manager will 
    /// return <see cref="LifetimeManager.NoValue"/>.</para>
    /// <para>Based on the registration type the behavior is different. If registered as Instance, out of 
    /// scope instance will create failed resolve but if registered as Factory or Type, the container will
    /// just create new instance.</para>
    /// <para>This lifetime manager is not disposed by the container.</para>
    /// </remarks>
    public class WeakReferenceLifetimeManager : LifetimeManager,
                                                IInstanceLifetimeManager,
                                                ITypeLifetimeManager,
                                                IFactoryLifetimeManager
    {
        #region Fields

        private WeakReference _value = new WeakReference(NoValue);

        #endregion


        #region Overrides

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            var value = _value.Target;

            return _value.IsAlive ? value : NoValue;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="container">Instance of container which owns the value</param>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            _value = new WeakReference(newValue);
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new WeakReferenceLifetimeManager();
        }

        public override string ToString() => "Lifetime:WeakReference";

        #endregion
    }
}
