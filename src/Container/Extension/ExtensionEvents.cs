
namespace Unity.Extension
{
    /// <summary>
    /// Registration event handler
    /// </summary>
    /// <param name="container">Container where the registration took place</param>
    /// <param name="registration">Reference to <see cref="RegistrationData"/> structure</param>
    public delegate void RegistrationEvent(object container, in RegistrationData registration);

    /// <summary>
    /// Child container created event handler
    /// </summary>
    /// <param name="parent">Container creating the child</param>
    /// <param name="child">Context of created child container</param>
    public delegate void ChildCreatedEvent(object parent, ExtensionContext child);
}

