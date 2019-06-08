using System;
using Unity.Resolution;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base class for all lifetime managers - classes that control how
    /// and when instances are created by the Unity container.
    /// </summary>
    public abstract class LifetimeManager 
    {
        /// <summary>
        /// This value represents Invalid Value. Lifetime manager must return this
        /// unless value is set with a valid object. Null is a value and is not equal 
        /// to NoValue 
        /// </summary>
        public static readonly object NoValue = new InvalidValue();

        /// <summary>
        /// A <see cref="Boolean"/> indicating if this manager is being used in 
        /// one of the registrations.
        /// </summary>
        /// <remarks>
        /// The Unity container requires that each registration used its own, unique
        /// lifetime manager. This property is being used to track that condition.
        /// </remarks>
        /// <value>True is this instance already in use, False otherwise.</value>
        public virtual bool InUse { get; set; }

        
        #region Constructors

        public LifetimeManager()
        {
            Set    = SetValue;
            Get    = GetValue;
            TryGet = TryGetValue;
        }
        
        #endregion


        #region  Optimizers

        public virtual Func<ILifetimeContainer, object> TryGet { get; protected set; }

        public virtual Func<ILifetimeContainer, object> Get { get; protected set; }

        public virtual Action<object, ILifetimeContainer> Set { get; protected set; }

        #endregion


        #region   LifetimeManager Members

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on synchronization 
        /// primitives.
        /// </remarks>
        /// <param name="container">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object TryGetValue(ILifetimeContainer container = null) => GetValue(container);

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object GetValue(ILifetimeContainer container = null) => NoValue;

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">The container this lifetime is associated with</param>
        public virtual void SetValue(object newValue, ILifetimeContainer container = null) { }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        /// <param name="container">The container this lifetime belongs to</param>
        public virtual void RemoveValue(ILifetimeContainer container = null) { }

        #endregion


        #region ILifetimeFactoryPolicy

        /// <summary>
        /// Creates a new lifetime manager of the same type as this Lifetime Manager
        /// </summary>
        /// <returns>A new instance of the appropriate lifetime manager</returns>
        public LifetimeManager CreateLifetimePolicy() => OnCreateLifetimeManager();

        #endregion


        #region Implementation

        /// <summary>
        /// Implementation of <see cref="CreateLifetimePolicy"/> policy.
        /// </summary>
        /// <returns>A new instance of the same lifetime manager of appropriate type</returns>
        protected abstract LifetimeManager OnCreateLifetimeManager();

        #endregion


        #region Nested Types

        public class InvalidValue
        {
            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion


        #region Internal Use

        internal Delegate PipelineDelegate;

        internal virtual object Pipeline<TContext>(ref TContext context) where TContext : IResolveContext 
            => ((ResolveDelegate<TContext>)PipelineDelegate)(ref context);

        #endregion


        #region Debugger
#if DEBUG
        public string ID { get; } = Guid.NewGuid().ToString();
#endif
        #endregion
    }
}
