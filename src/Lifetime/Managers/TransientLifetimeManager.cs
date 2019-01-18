namespace Unity.Lifetime
{
    /// <summary>
    /// An <see cref="LifetimeManager"/> implementation that does nothing,
    /// thus ensuring that instances are created new every time.
    /// </summary>
    public class TransientLifetimeManager : LifetimeManager,
                                            IFactoryLifetimeManager
    {
        public static TransientLifetimeManager Instance = new TransientLifetimeManager();

        public override bool InUse
        {
            get => false;
            set { }
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this WithLifetime policy.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return null;
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return Instance;
        }


        #region Overrides

        public override string ToString() => "WithLifetime.Transient";

        #endregion
    }
}
