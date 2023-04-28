using Unity.Lifetime;
using Unity.Storage;

namespace Unity.Container
{
    public class VersionControlledLifetimeManager : LifetimeManager
    {
        #region Fields

        //private int _version = -1;
        private readonly Scope _scope;

        #endregion


        #region Constructors

        public VersionControlledLifetimeManager(Scope scope)
            => _scope = scope;

        #endregion


        #region Pipeline


        #endregion


        #region Clone

        protected override LifetimeManager OnCreateLifetimeManager()
            => throw new NotSupportedException($"{nameof(VersionControlledLifetimeManager)} should not be cloned");

        #endregion
    }
}
