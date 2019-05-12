
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
    public class SingletonLifetimeManager : ContainerControlledLifetimeManager
    {
        #region Overrides
        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:Singleton";

        #endregion
    }
}
