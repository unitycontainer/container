using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public partial struct ResolutionContext
    {
        private static readonly RegistrationManager NoRegistration = new NoRegistrationManager();

        private class NoRegistrationManager : RegistrationManager
        {
            public override object? TryGetValue(ICollection<IDisposable> lifetime)
                => NoValue;
        }
    }
}
