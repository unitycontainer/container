using System;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Get and Check Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            return null == name
                ? _scope.IsRegistered(type)
                : _scope.IsRegistered(type, name);
        }

        #endregion
    }
}
