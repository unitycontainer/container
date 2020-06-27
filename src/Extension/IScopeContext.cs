using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Scope
{
    /// <summary>
    /// Local settings related to current container scope
    /// </summary>
    public interface IScopeContext : IPolicyList, IPolicySet
    {
        /// <summary>
        /// Default implicit lifetime of type registrations 
        /// </summary>
        ITypeLifetimeManager DefaultTypeLifetime { get; set; }

        /// <summary>
        /// Default implicit lifetime of factory registrations 
        /// </summary>
        IFactoryLifetimeManager DefaultFactoryLifetime { get; set; }

        /// <summary>
        /// Default implicit lifetime of instance registrations 
        /// </summary>
        IInstanceLifetimeManager DefaultInstanceLifetime { get; set; }
    }
}
