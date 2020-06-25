
namespace Unity.Extension
{
    /// <summary>
    /// Registration event handler
    /// </summary>
    /// <param name="registration">Reference to <see cref="RegistrationData"/> structure</param>
    public delegate void RegistrationEvent(ref RegistrationData registration);

    /// <summary>
    /// Child container created event handler
    /// </summary>
    /// <param name="child">Created child container</param>
    public delegate void ChildCreatedEvent(IUnityContainer child);
}
