using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity
{
    public partial class UnityContainer// : IEnumerable<ContainerRegistration>
    {
        public Enumerator GetEnumerator()
            => new Enumerator(this);

        //IEnumerator<ContainerRegistration> IEnumerable<ContainerRegistration>.GetEnumerator()
        //    => new Enumerator(this);

        //IEnumerator IEnumerable.GetEnumerator()
        //    => new Enumerator(this);
    }
}
