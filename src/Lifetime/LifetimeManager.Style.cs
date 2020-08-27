
namespace Unity.Lifetime
{
    #region Resolution Style

    /// <summary>
    /// Resolution styles of lifetime managers
    /// </summary>
    public enum ResolutionStyle
    { 
        /// <summary>
        /// The value is resolved once and stored with the manager 
        /// for the rest of manager's life. 
        /// </summary>
        /// <remarks>
        /// Building a pipeline for this type of manager is not recommended, 
        /// the value should be resolved via simple activation instead.
        /// </remarks>
        OnceInLifetime,

        /// <summary>
        /// The value is resolved and stored with the manager until invalidated. 
        /// Once invalidated, the value resolved and stored again.
        /// </summary>
        /// <remarks>
        /// This type of managers resolve values just occasionally and are not 
        /// vary sensitive to minor overhead during resolution. Instead speed 
        /// of pipeline creation takes priority.
        /// </remarks>
        OnceInWhile,

        /// <summary>
        /// The value is resolved almost every single time or is not stored
        /// </summary>
        /// <remarks>
        /// This type of managers (<see cref="TransientLifetimeManager"/> for example)
        /// resolve values very frequently and require very optimized pipeline. 
        /// For these the pipeline performance is prioritized over pipeline creation time.
        /// </remarks>
        EveryTime
    }
    
    #endregion


    /// <summary>
    /// Base class for all lifetime managers - classes that control how
    /// and when instances are created by the Unity container.
    /// </summary>
    public abstract partial class LifetimeManager : RegistrationManager
    {
        /// <summary>
        /// Resolution styles of lifetime managers
        /// </summary>
        /// <remarks>
        /// This method must be overwritten in derived managers to indicate
        /// preferred resolution strategy for the manager.
        /// </remarks>
        public abstract ResolutionStyle Style { get; }
    }
}
