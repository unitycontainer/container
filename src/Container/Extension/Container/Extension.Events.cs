
using System;

namespace Unity.Extension
{
    /// <summary>
    /// Registration event handler
    /// </summary>
    /// <param name="container">Container where the registration took place</param>
    /// <param name="registrations">Reference to <see cref="ReadOnlySpan{RegistrationDescriptor}"/> structure
    /// containing all registrations</param>
    public delegate void RegistrationEvent(object container, in ReadOnlySpan<RegistrationDescriptor> registrations);

    /// <summary>
    /// Child container created event handler
    /// </summary>
    /// <param name="parent">Container creating the child</param>
    /// <param name="child">Context of created child container</param>
    public delegate void ChildCreatedEvent(object parent, ExtensionContext child);
}

