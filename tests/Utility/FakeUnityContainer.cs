using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Abstractions.Tests
{
    public class FakeUnityContainer : IUnityContainer, IEnumerable
    {
        #region Public Members

        public Type Type { get; private set; }
        public string Name { get; private set; }
        public Type MappedTo { get; private set; }
        public LifetimeManager LifetimeManager { get; private set; }
        public ICollection<InjectionMember> InjectionMembers { get; private set; }
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


        public IEnumerable<IContainerRegistration> Registrations => throw new NotImplementedException();

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

        public IUnityContainer RegisterType(Type type, string name, ITypeLifetimeManager manager, params Type[] registerAs)
        {
            Type = registerAs.FirstOrDefault();
            MappedTo = type;
            Name = name;
            LifetimeManager = (LifetimeManager)manager;
            InjectionMembers = LifetimeManager.InjectionMembers;

            return this;
        }

        public IUnityContainer RegisterFactory(ResolveDelegate<IResolveContext> factory, string name, IFactoryLifetimeManager manager, params Type[] registerAs)
        {
            Type = registerAs.FirstOrDefault();
            Name = name;

            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (null == Type) throw new ArgumentNullException(nameof(Type));

            var context = _context as IResolveContext;
            Data = factory.Invoke(ref context);

            LifetimeManager = (LifetimeManager)manager;
            InjectionMembers = LifetimeManager.InjectionMembers;

            return this;
        }

        public IUnityContainer RegisterInstance(object instance, string name, IInstanceLifetimeManager manager, params Type[] registerAs)
        {
            Type = registerAs.FirstOrDefault();
            Name = name;
            Data = instance;

            LifetimeManager = (LifetimeManager)manager;
            InjectionMembers = LifetimeManager.InjectionMembers;

            return this;
        }
    }
}
