using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope : IAsyncEnumerable<ContainerRegistration>
        {
            public IAsyncEnumerator<ContainerRegistration> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
