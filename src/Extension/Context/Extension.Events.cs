using Unity.Lifetime;

namespace Unity.Extension;

/// <summary>
/// Registration event handler
/// </summary>
/// <param name="container">Container where the registration took place</param>
/// <param name="registrations">Reference to <see cref="ReadOnlySpan{RegistrationDescriptor}"/> structure
/// containing all registrations</param>
public delegate void RegistrationEvent(object container, Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager);

/// <summary>
/// Child container created event handler
/// </summary>
/// <param name="parent">Container creating the child</param>
/// <param name="child">Context of created child container</param>
public delegate void ChildCreatedEvent(object parent, ExtensionContext child);



public abstract partial class ExtensionContext
{
    /// <summary>
    /// This event is raised on new registration
    /// </summary>
    public abstract event RegistrationEvent Registering;

    /// <summary>
    /// This event is raised when the <see cref="IUnityContainer.CreateChildContainer"/> 
    /// method is called and new child container is created. It allow extensions to 
    /// perform any additional initialization they may require.
    /// </summary>
    public abstract event ChildCreatedEvent ChildContainerCreated;
}

