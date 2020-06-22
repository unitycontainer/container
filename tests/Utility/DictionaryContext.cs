using System;
using System.Collections.Generic;
using Unity.Resolution;

namespace Unity.Abstractions.Tests
{
    public class DictionaryContext : Dictionary<Type, object>, IResolveContext
    {
        public Func<Type, string, object> Resolver { get; set; }

        public DictionaryContext() => Resolver = DefaultResolver;


        public IUnityContainer Container => throw new NotImplementedException();

        public Type Type => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public void Clear(Type type, string name, Type policyInterface) => throw new NotImplementedException();

        public object Get(Type type, Type policyInterface) => throw new NotImplementedException();

        public object Get(Type type, string name, Type policyInterface) => throw new NotImplementedException();

        public object Resolve(Type type, string name) => Resolver(type, name);

        public void Set(Type type, Type policyInterface, object policy) => throw new NotImplementedException();

        public void Set(Type type, string name, Type policyInterface, object policy) => throw new NotImplementedException();

        private object DefaultResolver(Type type, string name)
        {
            try
            {
                return this[type];
            }
            catch
            {
                throw new ResolutionFailedException(type, name, $"{type} is not registered");
            }
        }
    }
}
