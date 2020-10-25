using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity
{
    public partial class UnityContainer
    {

        public struct Enumerator : IEnumerator<ContainerRegistration>
        {
            #region Fields

            UnityContainer _container;

            #endregion


            public Enumerator(UnityContainer container)
            {
                _container = container;
            }


            public ContainerRegistration Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
