namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        #region Policies


        public virtual bool IsLocal => false;

        /// <summary>
        /// Creation policy
        /// </summary>
        public virtual CreationPolicy CreationPolicy => CreationPolicy.Always;

        #endregion
    }


    // TODO: Rethink the concept


    /// <summary>
    /// Different approaches to creating lifetimes
    /// </summary>
    public enum CreationPolicy : int
    {
        /// <summary>
        /// Let the <see cref="Hosting.CompositionContainer"/> choose the most appropriate <see cref="CreationPolicy"/>
        /// for the part given the current context. This is the default <see cref="CreationPolicy"/>, with
        /// the <see cref="Hosting.CompositionContainer"/> choosing <see cref="CreationPolicy.Once"/> by default
        /// unless the <see cref="Primitives.ComposablePart"/> or importer requests <see cref="CreationPolicy.OnceInWhile"/>.
        /// </summary>
        Always = 0,

        /// <summary>
        /// A single shared instance of the associated <see cref="Primitives.ComposablePart"/> will be created
        /// by the <see cref="Hosting.CompositionContainer"/> and shared by all requestors.
        /// </summary>
        Once = 1,

        /// <summary>
        /// A new non-shared instance of the associated <see cref="Primitives.ComposablePart"/> will be created
        /// by the <see cref="Hosting.CompositionContainer"/> for every requestor.
        /// </summary>
        OnceInWhile = 2,
    }
}
