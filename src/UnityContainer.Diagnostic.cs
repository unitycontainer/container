using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Processors;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    [DebuggerDisplay("{" + nameof(DebugName) + "()}")]
    [DebuggerTypeProxy(typeof(UnityContainerDebugProxy))]
    public partial class UnityContainer
    {
        #region Fields

        private readonly ResolveDelegateFactory _buildStrategy = OptimizingFactory;

        #endregion

        internal enum BuildStrategy
        {
            Compiled,

            Resolved,

            Optimized
        }

        #region Diagnostic Constructor

        internal UnityContainer(BuildStrategy strategy)
            : this()
        {
            switch (strategy)
            {
                case BuildStrategy.Compiled:
                    _buildStrategy = CompilingFactory;
                    Defaults.Set(typeof(ResolveDelegateFactory), _buildStrategy);
                    break;

                case BuildStrategy.Resolved:
                    _buildStrategy = ResolvingFactory;
                    Defaults.Set(typeof(ResolveDelegateFactory), _buildStrategy);
                    break;
            }
        }


        #endregion

        private string DebugName()
        {
            var types = (_registrations?.Keys ?? Enumerable.Empty<Type>())
                .SelectMany(t => _registrations[t].Values)
                .OfType<ContainerRegistration>()
                .Count();

            if (null == _parent) return $"Container[{types}]";

            return _parent.DebugName() + $".Child[{types}]"; 
        }

        internal class UnityContainerDebugProxy
        {
            private readonly IUnityContainer _container;

            public UnityContainerDebugProxy(IUnityContainer container)
            {
                _container = container;
                Id = container.GetHashCode().ToString();
            }

            public string Id { get; }

            public IEnumerable<IContainerRegistration> Registrations => _container.Registrations;

        }
    }
}
