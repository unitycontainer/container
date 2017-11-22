using System;
using System.Collections.Generic;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container
{
    public interface IContext
    {
        IContext Parent { get; }

        IUnityContainer Container { get; }

        IEnumerable<IContainerRegistration> this[Type type] { get; }

        IContainerRegistration this[Type type, string name] { get; }

        IBuilderPolicy this[Type type, string name, Type policy] { get; set; }
    }
}
