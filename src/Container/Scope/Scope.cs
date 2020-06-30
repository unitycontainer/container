using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Scope
{
    public partial class RegistrationScope : IEnumerable<ContainerRegistration>
    {

        #region IEnumerable
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ContainerRegistration> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Disposables

        /// <inheritdoc />
        public ICollection<IDisposable> Lifetime { get; } 
            = new List<IDisposable>();

        #endregion
    }
}
