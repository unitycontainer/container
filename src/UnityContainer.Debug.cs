using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Registration;

namespace Unity
{
    [DebuggerDisplay("{DebugName()}")]
    [DebuggerTypeProxy(typeof(UnityContainerDebugProxy))]
    public partial class UnityContainer
    {
        private string DebugName()
        {
            var types = (_registrations?.Keys ?? Enumerable.Empty<Type>())
                .SelectMany(t => _registrations[t].Values)
                .OfType<ContainerRegistration>()
                .Count();

            if (null == _parent) return $"Container[{types}]";

            return _parent.DebugName() + $".Child[{types}]"; ;
        }


        internal class UnityContainerDebugProxy
        {
            private readonly UnityContainer _container;

            public UnityContainerDebugProxy(UnityContainer container)
            {
                _container = container;
                ID = container.GetHashCode().ToString();
            }

            public string ID { get; }

            public IEnumerable<IContainerRegistration> Registrations => _container.Registrations;

        }
    }
}
