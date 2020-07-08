using System;
using System.Collections.Generic;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            return null == name
                ? _scope.IsRegistered(type)
                : _scope.IsRegistered(type, name);
        }

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                return _scope.Registrations;
            }
        }

        #endregion
    }
}
