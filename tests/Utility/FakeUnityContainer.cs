using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Abstractions.Tests
{
    public class FakeUnityContainer : IUnityContainer, IEnumerable
    {
        #region Fields

        RegistrationDescriptor[] _descriptors = new[] { new RegistrationDescriptor() };

        #endregion

        #region Public Members

        public ref readonly RegistrationDescriptor Descriptor => ref _descriptors[0];

        public Type Type { get; private set; }

        public string Name { get; private set; }

        public ICollection<InjectionMember> InjectionMembers => _descriptors?[0].Manager.InjectionMembers;

        public ResolverOverride[] ResolverOverrides { get; private set; }

        public object Data { get; set; }

        #endregion


        #region Context

        private DictionaryContext _context;

        #endregion

        public Func<Type, object> Resolver { get; set; }


        public FakeUnityContainer()
        {
            _context = new DictionaryContext
            {
                Type = typeof(IUnityContainer)
            };

            Resolver = DefaultResolver;
        }

        public void Add(Type key, object value) => _context.Add(key, value);


        public IEnumerable<ContainerRegistration> Registrations => throw new NotImplementedException();

        public IUnityContainer Parent => throw new NotImplementedException();

        IEnumerable<ContainerRegistration> IUnityContainer.Registrations => throw new NotImplementedException();

        public object BuildUp(Type type, object existing, string name, params ResolverOverride[] overrides)
        {
            Type = type;
            Name = name;
            Data = existing;

            return Data;
        }

        public void Dispose() => throw new NotImplementedException();

        public bool IsRegistered(Type type, string name)
        {
            Type = type;
            Name = name;

            return true;
        }


        public object Resolve(Type type, string name, params ResolverOverride[] overrides)
        {
            Type = type;
            Name = name;
            ResolverOverrides = overrides;

            return Resolver(type);
        }

        private object DefaultResolver(Type type)
        {
            try
            {
                return _context[type];
            }
            catch
            {
                throw new ResolutionFailedException(type, null, $"{type} is not registered");
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _context.GetEnumerator();
        }

        public IUnityContainer CreateChildContainer(string name = null) => this;

        public IUnityContainer Register(params RegistrationDescriptor[] descriptors)
        {
            _descriptors = descriptors;

            return this;
        }

        public IUnityContainer Register(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            throw new NotImplementedException();
        }
    }
}
