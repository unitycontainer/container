using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Unity
{
    //[DebuggerDisplay("{" + nameof(DebugName) + "()}")]
    [DebuggerTypeProxy(typeof(UnityContainerDebugProxy))]
    public partial class UnityContainer
    {
        #region Debug Support

        // TODO: Re-implement
        //private string DebugName()
        //{
        //    var types = (_registrations?.Keys ?? Enumerable.Empty<Type>())
        //        .SelectMany(t => _registrations[t].Values)
        //        .OfType<ContainerRegistration>()
        //        .Count();

        //    if (null == _parent) return $"Container[{types}]";

        //    return _parent.DebugName() + $".Child[{types}]"; 
        //}

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

        #endregion
    }
}
