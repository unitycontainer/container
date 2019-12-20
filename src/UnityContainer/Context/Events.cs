using System;
using Unity.Events;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext : IExtensionContext
        {
            public event EventHandler<RegisterEventArgs> Registering
            {
                add => Container.Registering += value;
                remove => Container.Registering -= value;
            }

            public event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add => Container.RegisteringInstance += value;
                remove => Container.RegisteringInstance -= value;
            }

            public event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add => Container.ChildContainerCreated += value;
                remove => Container.ChildContainerCreated -= value;
            }
        }
    }
}
