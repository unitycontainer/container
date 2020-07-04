using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("Size = { Count }", Name = "Scope({ Container.Name })")]
    public partial class ContainerScope
    {
        #region Public Fields

        public readonly ContainerScope? Parent;
        public readonly UnityContainer Container;

        #endregion


        #region Public Properties

        public int Count => _registryCount - 3;

        #endregion
    }
}
