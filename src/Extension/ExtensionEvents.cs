
using Unity.Scope;

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
    /// <param name="child">Context of child container</param>
    public delegate void ChildCreatedEvent(IScopeContext child);
}
