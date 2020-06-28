using System;
using System.Collections.Generic;
using System.Text;

namespace Unity
{
    public partial class UnityContainer
    {

        #region Child Containers

        private UnityContainer CreateChildContainer(string? name = null)
        {
            var container = new UnityContainer(this, name);

            if (_rootContext.RaiseChildCreated)
                _rootContext.OnChildContainerCreated(_scopeContext ??= new ScopeContext(this));

            return container;
        }

        #endregion
    }
}
