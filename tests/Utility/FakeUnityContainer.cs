using System;
using System.Collections;
using System.Collections.Generic;
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
        public InjectionMember[] InjectionMembers { get; private set; }
        public ResolverOverride[] ResolverOverrides { get; private set; }
        public object Data { get; set; }

        #endregion


        #region Context

        private DictionaryContext _context;

        #endregion

        public Func<Type, object> Resolver { get; set; }


        public FakeUnityContainer()
        {
            _context = new DictionaryContext();

            Resolver = DefaultResolver;
        }

        public void Add(Type key, object value) => _context.Add(key, value);


        public IEnumerable<IContainerRegistration> Registrations => throw new NotImplementedException();

        public IUnityContainer Parent => throw new NotImplementedException();

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

        public IUnityContainer RegisterFactory(Type type, string name, Func<IResolveContext, object> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Type = type;
            Name = name;
            Data = factory;
            LifetimeManager = (LifetimeManager)lifetimeManager;
            InjectionMembers = injectionMembers;

            return this;
        }

        public IUnityContainer RegisterFactory(Type type, string name, ResolveDelegate<IResolveContext> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            Type = type;
            Name = name;
            
            var context = _context as IResolveContext;
            Data = factory.Invoke(ref context);

            LifetimeManager = (LifetimeManager)lifetimeManager;
            InjectionMembers = injectionMembers;

            return this;
        }

        public IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));

            Type = type;
            Name = name;
            Data = factory?.Invoke(this, type, name); 
            LifetimeManager = (LifetimeManager)lifetimeManager;
            InjectionMembers = injectionMembers;

            return this; 
        }

        public IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Type = type;
            Name = name;
            Data = instance;
            LifetimeManager = (LifetimeManager)lifetimeManager;
            InjectionMembers = injectionMembers;

            return this;
        }

        public IUnityContainer RegisterType(Type registeredType, Type mappedToType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Type = registeredType;
            MappedTo = mappedToType;
            Name = name;
            LifetimeManager = (LifetimeManager)lifetimeManager;
            InjectionMembers = injectionMembers;

            return this;
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

        public IUnityContainer CreateChildContainer() => throw new NotImplementedException();
        
        public IUnityContainer CreateChildContainer(string name = null)
        {
            throw new NotImplementedException();
        }
    }
}
