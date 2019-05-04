namespace Unity.Lifetime
{
    /// <summary>
    /// This interface marks all lifetime managers compatible with 
    /// <see cref="IUnityContainer.RegisterInstance" /> registration.
    /// </summary>
    /// <remarks>
    /// This interface is used for design type validation of registration compatibility.
    /// Each registration type only takes lifetime managers compatible with it. 
    /// </remarks>
    public interface IInstanceLifetimeManager
    {
        LifetimeManager CreateLifetimePolicy();
    }
}
