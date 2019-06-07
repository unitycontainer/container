using System;
using Unity.Events;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext : ExtensionContext
        {
            public override event EventHandler<RegisterEventArgs> Registering
            {
                add => Container.Registering += value;
                remove => Container.Registering -= value;
            }

            public override event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
            {
                add => Container.RegisteringInstance += value;
                remove => Container.RegisteringInstance -= value;
            }

            public override event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated
            {
                add => Container.ChildContainerCreated += value;
                remove => Container.ChildContainerCreated -= value;
            }
        }
    }
}
