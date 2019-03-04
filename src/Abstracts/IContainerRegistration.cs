using System;
using Unity.Lifetime;

namespace Unity
{
    /// <summary>
    /// Information about the types registered in a container.
    /// <para>
    /// Types registered with container are stored in registry and exposed to outside world
    /// through <see cref="IUnityContainer.Registrations"/> enumeration. Each record is 
    /// represented by an instance of this interface.
    /// </para>
    /// </summary>
    public interface IContainerRegistration
    {
        /// <summary>
        /// Type of the registration.
        /// </summary>
        /// <remarks>
        /// This <see cref="Type"/> is what container stores in its internal registry. When resolving, it will look 
        /// for this <see cref="Type"/> to satisfy dependencies. 
        /// </remarks>
        /// <value>The type registered with the container</value>
        Type RegisteredType { get; }

        /// <summary>
        /// Name the registered type. Null for default registration.
        /// </summary>
        /// <remarks>
        /// Each registration is uniquely identified by <see cref="RegisteredType"/> and <see cref="Name"/>. 
        /// Registering the same <see cref="Type"/> with different names will create distinct registrations 
        /// for each <see cref="RegisteredType"/> and <see cref="Name"/> combinations. Registration with no <see cref="Name"/> 
        /// (<c>name == null</c>) is called <c>default registration</c>. The container uses these as implicit defaults when required.
        /// </remarks>
        /// <value>Name of the registration</value>
        string Name { get; }

        /// <summary>
        /// The type that this registration is mapped to. 
        /// </summary>
        /// <remarks>
        /// Type <see cref="MappedToType"/> informs container how to build the requested instance. Based on how it was registered, the
        /// <see cref="Type"/> could be built from this registration or redirected to mapped registration to satisfy the request. 
        /// </remarks>
        /// <value>The type of object created when registered type is requested</value>
        Type MappedToType { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each Unity registration is assigned a lifetime manager that controls how 
        /// the container creates, manages, and disposes of created objects. Even if 
        /// manager is not assigned explicitly, Unity will pick default type
        /// and add it to the registration.
        /// </para>
        /// </remarks>
        /// <value>Instance of lifetime manager associated with the registration</value>
        /// <seealso cref="Lifetime"/>
        LifetimeManager LifetimeManager { get; }
    }
}
