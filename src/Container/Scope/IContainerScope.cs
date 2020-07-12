using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public interface IContainerScope : IDisposable
    {
        IContainerScope? Parent { get; }

        IEnumerable<ContainerRegistration> Registrations { get; }

        ICollection<IDisposable> Disposables { get; }

        void Register(ref RegistrationData data);

        bool IsRegistered(Type type);
        
        bool IsRegistered(Type type, string name);

        IContainerScope CreateChildScope(UnityContainer container);
    }
}
