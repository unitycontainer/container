
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerResolveLifetimeManager : LifetimeManager
    {
        protected object value;

        /// <summary>
        /// Construct a new <see cref="PerResolveLifetimeManager"/> object that does not
        /// itself manage an instance.
        /// </summary>
        public PerResolveLifetimeManager()
        {
            value = null;
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return value;
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerResolveLifetimeManager();
        }
    }
}
