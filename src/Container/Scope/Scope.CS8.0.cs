using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.Scope
{
    public partial class RegistrationScope : IAsyncEnumerable<ContainerRegistration>
    {
        public IAsyncEnumerator<ContainerRegistration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
